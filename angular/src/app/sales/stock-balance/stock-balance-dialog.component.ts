import { Component, Injector, OnInit, Inject } from "@angular/core";
import {
  forEach as _forEach,
  includes as _includes,
  map as _map,
} from "lodash-es";
import { AppComponentBase } from "@shared/app-component-base";
import {
  SalesServiceProxy,
  GroupBySellingPriceDto,
} from "@shared/service-proxies/service-proxies";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-stock-balance-dialog",
  templateUrl: "./stock-balance-dialog.component.html",
  styleUrls: ["./stock-balance-dialog.component.scss"],
})
export class StockBalanceDialogComponent
  extends AppComponentBase
  implements OnInit {
  displayedColumns: string[] = [
    "quantity",
    "selling-price",
    "actions",
  ];
  productStockBalances: GroupBySellingPriceDto[] = []; // GropuBy SellingPrice

  constructor(
    injector: Injector,
    private _salesService: SalesServiceProxy,
    public matDialogRef: MatDialogRef<StockBalanceDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._salesService
      .getStockBalancesByProductId(this.data.id)
      .subscribe((response) => {
        this.productStockBalances = response;
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
