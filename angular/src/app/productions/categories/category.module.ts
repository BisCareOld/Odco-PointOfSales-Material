import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { CategoriesComponent } from "./categories.component";
import { CreateCategoryDialogComponent } from "./create-category/create-category-dialog.component";
import { EditCategoryDialogComponent } from "./edit-category/edit-category-dialog.component";

@NgModule({
  declarations: [
    CategoriesComponent,
    CreateCategoryDialogComponent,
    EditCategoryDialogComponent,
  ],
  imports: [CommonModule, FormsModule, SharedModule],
  entryComponents: [CreateCategoryDialogComponent, EditCategoryDialogComponent],
})
export class CategoryModule {}
