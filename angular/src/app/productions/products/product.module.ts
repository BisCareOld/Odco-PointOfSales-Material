import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { ProductsComponent } from "./products.component";

@NgModule({
  declarations: [ProductsComponent],
  imports: [CommonModule, FormsModule, SharedModule],
})
export class ProductModule {}
