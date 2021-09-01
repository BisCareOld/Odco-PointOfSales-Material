import { Component, Injector, EventEmitter, Output } from "@angular/core";
import { finalize } from "rxjs/operators";
import { AppComponentBase } from "@shared/app-component-base";
import {
  ProductionServiceProxy,
  BrandDto,
  CreateBrandDto,
} from "@shared/service-proxies/service-proxies";
import { forEach as _forEach, map as _map } from "lodash-es";
import { MatDialogRef } from "@angular/material/dialog";

@Component({
  selector: "app-create-brand-dialog",
  templateUrl: "./create-brand-dialog.component.html",
  styleUrls: ["./create-brand-dialog.component.scss"],
})
export class CreateBrandDialogComponent extends AppComponentBase {
  saving = false;
  brand = new BrandDto();

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    public matDialogRef: MatDialogRef<CreateBrandDialogComponent>
  ) {
    super(injector);
  }

  save(): void {
    this.saving = true;

    const _brand = new CreateBrandDto();
    _brand.init(this.brand);

    this._productionService
      .createBrand(_brand)
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
