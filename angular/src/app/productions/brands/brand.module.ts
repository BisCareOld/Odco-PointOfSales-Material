import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { BrandsComponent } from "./brands.component";
import { CreateBrandDialogComponent } from "./create-brand/create-brand-dialog.component";
import { EditBrandDialogComponent } from "./edit-brand/edit-brand-dialog.component";

@NgModule({
  declarations: [
    BrandsComponent,
    CreateBrandDialogComponent,
    EditBrandDialogComponent,
  ],
  imports: [CommonModule, FormsModule, SharedModule],
  entryComponents: [CreateBrandDialogComponent, EditBrandDialogComponent],
})
export class BrandModule {}
