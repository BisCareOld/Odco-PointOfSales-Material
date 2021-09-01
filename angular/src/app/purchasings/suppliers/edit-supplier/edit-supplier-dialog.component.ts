import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
  Inject,
} from "@angular/core";
import { finalize } from "rxjs/operators";
import {
  forEach as _forEach,
  includes as _includes,
  map as _map,
} from "lodash-es";
import { AppComponentBase } from "@shared/app-component-base";
import {
  PurchasingServiceProxy,
  SupplierDto,
} from "@shared/service-proxies/service-proxies";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-edit-supplier-dialog",
  templateUrl: "./edit-supplier-dialog.component.html",
  styleUrls: ["./edit-supplier-dialog.component.scss"],
})
export class EditSupplierDialogComponent
  extends AppComponentBase
  implements OnInit
{
  saving = false;
  supplier = new SupplierDto();

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _purchasingService: PurchasingServiceProxy,
    public matDialogRef: MatDialogRef<EditSupplierDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._purchasingService
      .getSupplier(this.data.id)
      .subscribe((result: SupplierDto) => {
        this.supplier = result;
      });
  }

  save(): void {
    this.saving = true;

    const _supplier = new SupplierDto();
    _supplier.init(this.supplier);

    this._purchasingService
      .updateSupplier(_supplier)
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
