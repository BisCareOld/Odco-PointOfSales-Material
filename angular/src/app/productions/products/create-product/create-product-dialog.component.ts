import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
} from "@angular/core";
import { finalize } from "rxjs/operators";
import { AppComponentBase } from "@shared/app-component-base";
import {
  ProductionServiceProxy,
  ProductDto,
  CreateProductDto,
} from "@shared/service-proxies/service-proxies";
import { forEach as _forEach, map as _map } from "lodash-es";
import { MatDialogRef } from "@angular/material/dialog";

@Component({
  selector: "app-create-product-dialog",
  templateUrl: "./create-product-dialog.component.html",
  styleUrls: ["./create-product-dialog.component.scss"],
})
export class CreateProductDialogComponent
  extends AppComponentBase
  implements OnInit
{
  saving = false;
  product = new ProductDto();

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    public matDialogRef: MatDialogRef<CreateProductDialogComponent>
  ) {
    super(injector);
  }

  ngOnInit(): void {}

  save(): void {
    this.saving = true;

    const product = new CreateProductDto();
    product.init(this.product);

    this._productionService
      .createProduct(product)
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
