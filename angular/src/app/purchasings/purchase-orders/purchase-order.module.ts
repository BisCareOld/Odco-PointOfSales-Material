import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { SharedModule } from "@shared/shared.module";
import { PurchaseOrdersComponent } from "./purchase-orders.component";
import { CreatePurchaseOrderComponent } from "./create-purchase-order/create-purchase-order.component";

@NgModule({
  declarations: [PurchaseOrdersComponent, CreatePurchaseOrderComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, SharedModule],
})
export class PurchaseOrderModule {}
