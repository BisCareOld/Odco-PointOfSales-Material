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
  ProductionServiceProxy,
  BrandDto,
} from "@shared/service-proxies/service-proxies";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-edit-brand-dialog",
  templateUrl: "./edit-brand-dialog.component.html",
  styleUrls: ["./edit-brand-dialog.component.scss"],
})
export class EditBrandDialogComponent
  extends AppComponentBase
  implements OnInit
{
  saving = false;
  brand = new BrandDto();

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    public matDialogRef: MatDialogRef<EditBrandDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._productionService
      .getBrand(this.data.id)
      .subscribe((result: BrandDto) => {
        this.brand = result;
      });
  }

  save(): void {
    this.saving = true;

    const _brand = new BrandDto();
    _brand.init(this.brand);

    this._productionService
      .updateBrand(_brand)
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
