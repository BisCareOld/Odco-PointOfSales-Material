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
  ProductDto,
  UpdateProductDto,
} from "@shared/service-proxies/service-proxies";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-edit-product-dialog",
  templateUrl: "./edit-product-dialog.component.html",
  styleUrls: ["./edit-product-dialog.component.scss"],
})
export class EditProductDialogComponent
  extends AppComponentBase
  implements OnInit
{
  saving = false;
  product = new ProductDto();

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    public matDialogRef: MatDialogRef<EditProductDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._productionService
      .getProduct(this.data.id)
      .subscribe((result: ProductDto) => {
        this.product = result;
      });
  }

  save(): void {
    this.saving = true;

    const _product = new UpdateProductDto();
    _product.init(this.product);

    this._productionService
      .updateProduct(_product)
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
