import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateOutstandingPaymentComponent } from './create-payment-for-customer-outstanding/create-outstanding-payment.component';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { PaymentsComponent } from './payments.component';

@NgModule({
  declarations: [
    CreateOutstandingPaymentComponent,
    PaymentsComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
  ]
})
export class PaymentModule { }
