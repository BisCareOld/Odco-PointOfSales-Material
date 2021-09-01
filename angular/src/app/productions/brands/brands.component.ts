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
  BrandDto,
  BrandDtoPagedResultDto,
} from "@shared/service-proxies/service-proxies";
import { CreateBrandDialogComponent } from "./create-brand/create-brand-dialog.component";
import { EditBrandDialogComponent } from "./edit-brand/edit-brand-dialog.component";

class PagedBrandRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

@Component({
  selector: "app-brands",
  templateUrl: "./brands.component.html",
  styleUrls: ["./brands.component.scss"],
  animations: [appModuleAnimation()],
})
export class BrandsComponent extends PagedListingComponentBase<BrandDto> {
  brands: BrandDto[] = [];
  keyword = "";
  isActive: boolean | null;
  advancedFiltersVisible = false;

  displayedColumns: string[] = ["name", "is-active", "actions"];
  dataSource;

  constructor(
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    private _matDialogService: MatDialog
  ) {
    super(injector);
  }

  protected list(
    request: PagedBrandRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._productionService
      .getAllBrands(
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
      .subscribe((result: BrandDtoPagedResultDto) => {
        this.brands = result.items;
        this.dataSource = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  delete(product: BrandDto): void {
    abp.message.confirm(
      this.l("RoleDeleteWarningMessage", product.name),
      undefined,
      (result: boolean) => {
        if (result) {
          this._productionService
            .deleteBrand(product.id)
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

  createBrand(): void {
    this.showCreateOrEditBrandDialog();
  }

  editBrand(category: BrandDto): void {
    this.showCreateOrEditBrandDialog(category.id);
  }

  showCreateOrEditBrandDialog(id?: string): void {
    let materialDialog;
    if (!id) {
      materialDialog = this._matDialogService.open(CreateBrandDialogComponent, {
        width: "50%",
      });
    } else {
      materialDialog = this._matDialogService.open(EditBrandDialogComponent, {
        data: { id: id },
        width: "50%",
      });
    }
    materialDialog.afterClosed().subscribe(() => {
      this.refresh();
    });
  }
}
