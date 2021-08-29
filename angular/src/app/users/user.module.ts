import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";
import { MaterialModule } from "@shared/material/material.module";

// users
import { UsersComponent } from "./users.component";
import { CreateUserDialogComponent } from "./create-user/create-user-dialog.component";
import { EditUserDialogComponent } from "./edit-user/edit-user-dialog.component";
import { ChangePasswordComponent } from "./change-password/change-password.component";
import { ResetPasswordDialogComponent } from "./reset-password/reset-password.component";

@NgModule({
  declarations: [
    // users
    UsersComponent,
    CreateUserDialogComponent,
    EditUserDialogComponent,
    ChangePasswordComponent,
    ResetPasswordDialogComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
  ],
  entryComponents: [
    // users
    CreateUserDialogComponent,
    EditUserDialogComponent,
    ResetPasswordDialogComponent,
  ],
})
export class UserModule {}
