import { Component, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "shared/paged-listing-component-base";
import {
  ProductionServiceProxy,
  ProductDto,
  ProductDtoPagedResultDto,
} from "@shared/service-proxies/service-proxies";

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
    private _productService: ProductionServiceProxy
  ) {
    super(injector);
  }

  createProduct(): void {}

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

  protected delete(entity: ProductDto): void {
    throw new Error("Method not implemented.");
  }
}
