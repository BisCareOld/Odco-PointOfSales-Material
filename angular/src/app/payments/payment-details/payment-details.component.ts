import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { FinanceServiceProxy, SalesServiceProxy, PaymentDto, PaymentLineLevelDto, CustomerDto, CustomerOutstandingSettlementDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-payment-details',
  templateUrl: './payment-details.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ['./payment-details.component.scss']
})
export class PaymentDetailsComponent extends AppComponentBase implements OnInit {
  paymentId: string;
  payment: PaymentDto;
  displayedColumns: string[] = ['position', 'type', 'received-amount', 'balance-amount', 'paid-amount'];
  dataSource: PaymentLineLevelDto[] = [];
  customer: CustomerDto;

  // Table: Customer Outstanding Settlement
  displayedSettlementColumns: string[] = ['position', 'customer', 'sale-number', 'invoice-number', 'paid-amount'];
  dataSourceSettlement: CustomerOutstandingSettlementDto[] = [];

  constructor(
    injector: Injector,
    private _financeService: FinanceServiceProxy,
    private _salesService: SalesServiceProxy,
    private router: Router,
    private route: ActivatedRoute) {
    super(injector);
  }

  ngOnInit(): void {
    this.paymentId = this.route.snapshot.queryParamMap.get("paymentId");

    if (this.isNullOrEmptyString(this.paymentId)) {
      this.notify.error(this.l("NoPaymentIdIsAvailable"));
    } else {
      this.getPaymentdetails();
    }
  }

  getPaymentdetails() {
    this._financeService.getPayment(this.paymentId).subscribe((result: PaymentDto) => {
      this.payment = result;

      if (this.payment.customerId) this.getCustomerDetails(this.payment.customerId);

      if (this.payment.paymentLineLevels.length > 0)
        this.dataSource = this.payment.paymentLineLevels;

      this.getCustmerOutstandingSettlements();
    });
  }

  getCustomerDetails(id) {
    this._salesService.getCustomer(id).subscribe((result) => {
      this.customer = result;
    });
  }

  getCustmerOutstandingSettlements() {
    if (this.payment.paymentType == 2) {
      this._financeService.getCustomerOutstandingSettlementsByPaymentId(this.paymentId).subscribe((result) => {
        this.dataSourceSettlement = result;
      });
    }
  }

}
