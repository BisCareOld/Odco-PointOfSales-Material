import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
  Inject,
} from "@angular/core";
import { finalize } from "rxjs/operators";
import { AppComponentBase } from "@shared/app-component-base";
import {
  TenantServiceProxy,
  TenantDto,
} from "@shared/service-proxies/service-proxies";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-edit-tenant-dialog",
  templateUrl: "./edit-tenant-dialog.component.html",
  styleUrls: ["./edit-tenant-dialog.component.scss"],
})
export class EditTenantDialogComponent
  extends AppComponentBase
  implements OnInit
{
  saving = false;
  tenant: TenantDto = new TenantDto();
  id: number;
  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public _tenantService: TenantServiceProxy,
    public matDialogRef: MatDialogRef<EditTenantDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._tenantService.get(this.data.id).subscribe((result: TenantDto) => {
      this.tenant = result;
    });
  }

  save(): void {
    this.saving = true;

    this._tenantService
      .update(this.tenant)
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
