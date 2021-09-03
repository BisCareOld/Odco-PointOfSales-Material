import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { InventoryTransactionsComponent } from "./inventory-transactions.component";
import { CreateInventoryTransactionsComponent } from "./create-inventory-transaction/create-inventory-transactions.component";

@NgModule({
  declarations: [
    InventoryTransactionsComponent,
    CreateInventoryTransactionsComponent,
  ],
  imports: [CommonModule, FormsModule, SharedModule],
})
export class InventoryTransactionModule {}
