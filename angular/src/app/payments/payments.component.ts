import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { FinanceServiceProxy, PaymentDto, PaymentDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

class PagedPaymentsRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}
@Component({
  templateUrl: './payments.component.html',
  styleUrls: ['./payments.component.scss'],
  animations: [appModuleAnimation()],
})
export class PaymentsComponent extends PagedListingComponentBase<PaymentDto> {
  keyword = "";
  isActive: boolean | null;
  advancedFiltersVisible = false;
  displayedColumns: string[] = [
    "sales-number",
    "invoice-number",
    "customer",
    "type",
    "is-outstanding-payment-involved",
    "paid-amount"
    // "net-amount",
    // "payment-status",
    // "actions",
  ];
  dataSource;

  constructor(
    injector: Injector,
    private _financesService: FinanceServiceProxy
  ) {
    super(injector);
  }

  protected list(
    request: PagedPaymentsRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._financesService
      .getAllPayments(
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
      .subscribe((result: PaymentDtoPagedResultDto) => {
        this.dataSource = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  protected delete(entity: PaymentDto): void {
    throw new Error('Method not implemented.');
  }

  pageChanges($event) {
    this.pageSize = $event.pageSize;
    this.getDataPage($event.pageIndex);
  }

  isNullOrEmpty(str: string): boolean {
    if (str != undefined && str != null && str != "") return false;

    return true;
  }

}
