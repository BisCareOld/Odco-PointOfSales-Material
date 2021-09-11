import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { InventoryTransactionsComponent } from "./inventory-transactions.component";
import { CreateInventoryTransactionsComponent } from "./create-inventory-transaction/create-inventory-transactions.component";
import { DirectInventoryTransactionProductComponent } from "./direct-inventory-transaction-product/direct-inventory-transaction-product.component";

@NgModule({
  declarations: [
    InventoryTransactionsComponent,
    CreateInventoryTransactionsComponent,
    DirectInventoryTransactionProductComponent,
  ],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, SharedModule],
})
export class InventoryTransactionModule {}
