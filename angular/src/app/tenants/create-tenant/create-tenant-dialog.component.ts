import {
  Component,
  Injector,
  OnInit,
  Output,
  EventEmitter,
} from "@angular/core";
import { finalize } from "rxjs/operators";
import { AppComponentBase } from "@shared/app-component-base";
import {
  CreateTenantDto,
  TenantServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { MatDialogRef } from "@angular/material/dialog";

@Component({
  selector: "app-create-tenant-dialog",
  templateUrl: "./create-tenant-dialog.component.html",
  styleUrls: ["./create-tenant-dialog.component.scss"],
})
export class CreateTenantDialogComponent
  extends AppComponentBase
  implements OnInit
{
  saving = false;
  tenant: CreateTenantDto = new CreateTenantDto();

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public _tenantService: TenantServiceProxy,
    public matDialogRef: MatDialogRef<CreateTenantDialogComponent>
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.tenant.isActive = true;
  }

  save(): void {
    this.saving = true;

    this._tenantService
      .create(this.tenant)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l("SavedSuccessfully"));
        this.matDialogRef.close();
        this.onSave.emit();
      });
  }
}
