import { Component, Injector, OnInit, Inject } from "@angular/core";
import {
  forEach as _forEach,
  includes as _includes,
  map as _map,
} from "lodash-es";
import { AppComponentBase } from "@shared/app-component-base";
import {
  ProductionServiceProxy,
  ProductStockBalanceDto,
} from "@shared/service-proxies/service-proxies";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-stock-balance-dialog",
  templateUrl: "./stock-balance-dialog.component.html",
  styleUrls: ["./stock-balance-dialog.component.scss"],
})
export class StockBalanceDialogComponent
  extends AppComponentBase
  implements OnInit
{
  displayedColumns: string[] = [
    "batch-number",
    "expiry-date",
    "quantity",
    "selling-price",
    "actions",
  ];
  productStockBalances: ProductStockBalanceDto[] = [];

  constructor(
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    public matDialogRef: MatDialogRef<StockBalanceDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._productionService
      .getStockBalancesByProductId(this.data.id)
      .subscribe((response) => {
        if (response.statusCode != 200)
          this.notify.info(response.message, "404");
        this.productStockBalances = response.items;
      });
  }

  submit() {
    console.log(this.productStockBalances.filter((p) => p.isSelected));
    this.matDialogRef.close({
      data: this.productStockBalances.filter((p) => p.isSelected),
      event: "SelectedProduct",
    });
  }

  cancel() {
    this.matDialogRef.close({
      event: "DialogClose",
    });
  }
}
