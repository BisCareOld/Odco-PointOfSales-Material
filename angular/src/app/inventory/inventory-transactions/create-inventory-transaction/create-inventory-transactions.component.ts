import { Component, OnInit, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { AppComponentBase } from "@shared/app-component-base";
import {
  ProductionServiceProxy,
  InventoryServiceProxy,
  CreateGoodsReceivedDto,
} from "@shared/service-proxies/service-proxies";
import { forEach as _forEach, map as _map } from "lodash-es";
import { appModuleAnimation } from "@shared/animations/routerTransition";

@Component({
  selector: "app-create-inventory-transactions",
  templateUrl: "./create-inventory-transactions.component.html",
  styleUrls: ["./create-inventory-transactions.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateInventoryTransactionsComponent
  extends AppComponentBase
  implements OnInit
{
  saving = false;
  grn = new CreateGoodsReceivedDto();

  constructor(
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    private _inventoryService: InventoryServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {}

  selectedSupplier($event) {
    console.log($event);
  }

  selectedProducts($event) {
    console.log($event);
  }

  save(): void {
    this.saving = true;

    const _grn = new CreateGoodsReceivedDto();
    _grn.init(this.grn);

    this._inventoryService
      .createGoodsReceivedNote(_grn)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l("SavedSuccessfully"));
      });
  }
}
