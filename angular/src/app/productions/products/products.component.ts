import { Component, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "shared/paged-listing-component-base";
import { MatDialog } from "@angular/material/dialog";
import {
  ProductionServiceProxy,
  ProductDto,
  ProductDtoPagedResultDto,
} from "@shared/service-proxies/service-proxies";
import { CreateProductDialogComponent } from "./create-product/create-product-dialog.component";
import { EditProductDialogComponent } from "./edit-product/edit-product-dialog.component";

class PagedProductRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

@Component({
  selector: "app-products",
  templateUrl: "./products.component.html",
  styleUrls: ["./products.component.scss"],
  animations: [appModuleAnimation()],
})
export class ProductsComponent extends PagedListingComponentBase<ProductDto> {
  product: ProductDto[] = [];
  keyword = "";
  isActive: boolean | null;
  advancedFiltersVisible = false;

  displayedColumns: string[] = [
    "code",
    "name",
    "category",
    "is-active",
    "actions",
  ];
  dataSource;

  constructor(
    injector: Injector,
    private _productService: ProductionServiceProxy,
    private _matDialogService: MatDialog
  ) {
    super(injector);
  }

  protected list(
    request: PagedProductRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._productService
      .getAllProducts(
        request.keyword,
        request.isActive,
        request.skipCount,
        request.maxResultCount
      )
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: ProductDtoPagedResultDto) => {
        this.product = result.items;
        this.dataSource = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  delete(product: ProductDto): void {
    abp.message.confirm(
      this.l("RoleDeleteWarningMessage", product.name),
      undefined,
      (result: boolean) => {
        if (result) {
          this._productService
            .deleteProduct(product.id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.l("SuccessfullyDeleted"));
                this.refresh();
              })
            )
            .subscribe(() => {});
        }
      }
    );
  }

  createProduct(): void {
    this.showCreateOrEditProductDialog();
  }

  editProduct(product: ProductDto): void {
    this.showCreateOrEditProductDialog(product.id);
  }

  showCreateOrEditProductDialog(id?: string): void {
    let materialDialog;
    if (!id) {
      materialDialog = this._matDialogService.open(
        CreateProductDialogComponent,
        {
          width: "70%",
        }
      );
    } else {
      materialDialog = this._matDialogService.open(EditProductDialogComponent, {
        data: { id: id },
        width: "70%",
      });
    }
    materialDialog.afterClosed().subscribe(() => {
      this.refresh();
    });
  }
}
