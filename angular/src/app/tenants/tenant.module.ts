import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";

import { SharedModule } from "@shared/shared.module";

import { TenantsComponent } from "./tenants.component";
// import { CreateTenantDialogComponent } from "./tenants/create-tenant/create-tenant-dialog.component";
// import { EditTenantDialogComponent } from "./tenants/edit-tenant/edit-tenant-dialog.component";

@NgModule({
  declarations: [
    // tenants
    TenantsComponent,
    // CreateTenantDialogComponent,
    // EditTenantDialogComponent,
  ],
  imports: [CommonModule, FormsModule, SharedModule],
  entryComponents: [
    // tenants
    // CreateTenantDialogComponent,
    // EditTenantDialogComponent,
  ],
})
export class TenantModule {}
