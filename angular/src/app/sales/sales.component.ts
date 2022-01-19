import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { SaleDto, SaleDtoPagedResultDto, SalesServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

class PagedSalesRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

@Component({
  selector: "app-sales",
  templateUrl: "./sales.component.html",
  styleUrls: ["./sales.component.scss"],
  animations: [appModuleAnimation()]
})
export class SalesComponent extends PagedListingComponentBase<SaleDto> {
  keyword = "";
  isActive: boolean | null;
  advancedFiltersVisible = false;
  skipCount: number = 0;

  displayedColumns: string[] = [
    "no",
    "sales-number",
    "customer",
    "net-amount",
    "payment-status"
  ];
  dataSource;

  constructor(
    injector: Injector,
    private _salesService: SalesServiceProxy
  ) {
    super(injector);
  }

  protected list(
    request: PagedSalesRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this.skipCount = request.skipCount;
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._salesService
      .getAllSales(
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
      .subscribe((result: SaleDtoPagedResultDto) => {
        this.dataSource = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  protected delete(entity: SaleDto): void {
    throw new Error('Method not implemented.');
  }

  pageChanges($event) {
    this.pageSize = $event.pageSize;
    this.getDataPage($event.pageIndex);
  }
}
