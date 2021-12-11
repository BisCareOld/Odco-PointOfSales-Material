import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { SalesComponent } from "./sales.component";
import { PaymentPanelComponent } from "./payment-panel/payment-panel.component";
import { StockBalanceDialogComponent } from "./stock-balance/stock-balance-dialog.component";
import { ChequeDialogComponent } from "./payment-methods/cheque/cheque-dialog.component";
import { CreateNonInventoryProductDialogComponent } from './create-non-inventory-product/create-non-inventory-product-dialog.component';

@NgModule({
  declarations: [
    SalesComponent,
    PaymentPanelComponent,
    StockBalanceDialogComponent,
    ChequeDialogComponent,
    CreateNonInventoryProductDialogComponent,
    CreateNonInventoryProductDialogComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
  ],
  entryComponents: [StockBalanceDialogComponent, ChequeDialogComponent, CreateNonInventoryProductDialogComponent],
})
export class SaleModule { }
