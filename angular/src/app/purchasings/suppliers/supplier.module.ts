import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { SuppliersComponent } from "./suppliers.component";
import { EditSupplierDialogComponent } from "./edit-supplier/edit-supplier-dialog.component";
import { CreateSupplierDialogComponent } from "./create-supplier/create-supplier-dialog.component";

@NgModule({
  declarations: [
    SuppliersComponent,
    EditSupplierDialogComponent,
    CreateSupplierDialogComponent,
  ],
  imports: [CommonModule, FormsModule, SharedModule, ReactiveFormsModule],
  entryComponents: [EditSupplierDialogComponent, CreateSupplierDialogComponent],
})
export class SupplierModule {}
