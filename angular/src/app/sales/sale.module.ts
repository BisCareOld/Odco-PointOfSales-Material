import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { SalesComponent } from "./sales.component";
import { CreateSalesComponent } from './create-sales/create-sales.component';
import { CreatePaymentComponent } from "./create-payment/create-payment.component";
import { StockBalanceDialogComponent } from "./stock-balance/stock-balance-dialog.component";
import { CreateNonInventoryProductDialogComponent } from './create-non-inventory-product/create-non-inventory-product-dialog.component';
import { SalesDetailsComponent } from './sales-details/sales-details.component';

@NgModule({
  declarations: [
    SalesComponent,
    CreatePaymentComponent,
    StockBalanceDialogComponent,
    CreateNonInventoryProductDialogComponent,
    CreateSalesComponent,
    SalesDetailsComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
  ],
  entryComponents: [
    StockBalanceDialogComponent,
    CreateNonInventoryProductDialogComponent,
    SalesComponent
  ],
})
export class SaleModule { }
