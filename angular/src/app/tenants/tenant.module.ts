import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { TenantsComponent } from "./tenants.component";
import { EditTenantDialogComponent } from "./edit-tenant/edit-tenant-dialog.component";
import { CreateTenantDialogComponent } from "./create-tenant/create-tenant-dialog.component";

@NgModule({
  declarations: [
    // tenants
    TenantsComponent,
    EditTenantDialogComponent,
    CreateTenantDialogComponent,
  ],
  imports: [CommonModule, FormsModule, SharedModule],
  entryComponents: [
    // tenants
    CreateTenantDialogComponent,
    EditTenantDialogComponent,
  ],
})
export class TenantModule {}
