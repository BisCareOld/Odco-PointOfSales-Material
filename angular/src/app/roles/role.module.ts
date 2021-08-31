import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { RolesComponent } from "./roles.component";
import { CreateRoleDialogComponent } from "./create-role/create-role-dialog.component";
import { EditRoleDialogComponent } from "./edit-role/edit-role-dialog.component";

@NgModule({
  declarations: [
    // roles
    RolesComponent,
    CreateRoleDialogComponent,
    EditRoleDialogComponent,
  ],
  imports: [CommonModule, FormsModule, SharedModule],
  entryComponents: [
    // roles
    CreateRoleDialogComponent,
    EditRoleDialogComponent,
  ],
})
export class RoleModule {}
