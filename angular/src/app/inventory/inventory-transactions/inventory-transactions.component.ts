import { Component, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "shared/paged-listing-component-base";
import { MatDialog } from "@angular/material/dialog";
import {
  InventoryServiceProxy,
  GoodsReceivedDto,
  GoodsReceivedDtoPagedResultDto,
} from "@shared/service-proxies/service-proxies";

class PagedGRNRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

@Component({
  selector: "app-inventory-transactions",
  templateUrl: "./inventory-transactions.component.html",
  styleUrls: ["./inventory-transactions.component.scss"],
  animations: [appModuleAnimation()],
})
export class InventoryTransactionsComponent extends PagedListingComponentBase<GoodsReceivedDto> {
  keyword = "";
  isActive: boolean | null;
  advancedFiltersVisible = false;

  displayedColumns: string[] = [
    "goods-received-number",
    "supplier-name",
    "discount-amount",
    "gross-amount",
    "net-amount",
    "actions",
  ];
  dataSource;

  constructor(
    injector: Injector,
    private _inventoryService: InventoryServiceProxy,
    private _matDialogService: MatDialog
  ) {
    super(injector);
  }
  protected list(
    request: PagedGRNRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._inventoryService
      .getAllGoodsReceivedProducts(
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
      .subscribe((result: GoodsReceivedDtoPagedResultDto) => {
        this.dataSource = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  protected delete(entity: GoodsReceivedDto): void {
    throw new Error("Method not implemented.");
  }

  pageChanges($event) {
    this.pageSize = $event.pageSize;
    this.getDataPage($event.pageIndex);
  }

  detailPage(grn: GoodsReceivedDto): void {}
}
