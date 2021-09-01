import { Component, Injector, EventEmitter, Output } from "@angular/core";
import { finalize } from "rxjs/operators";
import { AppComponentBase } from "@shared/app-component-base";
import {
  PurchasingServiceProxy,
  SupplierDto,
  CreateSupplierDto,
} from "@shared/service-proxies/service-proxies";
import { forEach as _forEach, map as _map } from "lodash-es";
import { MatDialogRef } from "@angular/material/dialog";

@Component({
  selector: "app-create-supplier-dialog",
  templateUrl: "./create-supplier-dialog.component.html",
  styleUrls: ["./create-supplier-dialog.component.scss"],
})
export class CreateSupplierDialogComponent extends AppComponentBase {
  saving = false;
  supplier = new SupplierDto();

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _purchasingService: PurchasingServiceProxy,
    public matDialogRef: MatDialogRef<CreateSupplierDialogComponent>
  ) {
    super(injector);
  }

  save(): void {
    this.saving = true;

    const _supplier = new CreateSupplierDto();
    _supplier.init(this.supplier);

    this._purchasingService
      .createSupplier(_supplier)
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
