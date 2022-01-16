import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { FinanceServiceProxy, PaymentDto, PaymentLineLevelDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-payment-details',
  templateUrl: './payment-details.component.html',
  styleUrls: ['./payment-details.component.scss']
})
export class PaymentDetailsComponent extends AppComponentBase implements OnInit {
  paymentId: string;
  payment: PaymentDto;
  displayedColumns: string[] = ['position', 'type', 'received-amount', 'balance-amount', 'paid-amount'];
  dataSource: PaymentLineLevelDto[] = [];

  constructor(
    injector: Injector,
    private _financeService: FinanceServiceProxy,
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

      if (this.payment.paymentLineLevels.length > 0)
        this.dataSource = this.payment.paymentLineLevels;

      console.log(result);
    })
  }

}
