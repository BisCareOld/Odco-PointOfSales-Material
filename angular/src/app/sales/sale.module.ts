import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { SalesComponent } from "./sales.component";

@NgModule({
  declarations: [SalesComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, SharedModule],
})
export class SaleModule {}
