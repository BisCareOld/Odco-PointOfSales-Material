import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { SalesComponent } from "./sales.component";
import { PaymentPanelComponent } from "./payment-panel/payment-panel.component";

@NgModule({
  declarations: [SalesComponent, PaymentPanelComponent],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
  ],
})
export class SaleModule {}
