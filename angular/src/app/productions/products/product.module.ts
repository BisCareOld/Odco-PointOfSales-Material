import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { ProductsComponent } from "./products.component";
import { CreateProductDialogComponent } from "./create-product/create-product-dialog.component";
import { EditProductDialogComponent } from "./edit-product/edit-product-dialog.component";

@NgModule({
  declarations: [
    ProductsComponent,
    CreateProductDialogComponent,
    EditProductDialogComponent,
  ],
  imports: [CommonModule, FormsModule, SharedModule],
  entryComponents: [CreateProductDialogComponent, EditProductDialogComponent],
})
export class ProductModule {}
