import { Component, OnInit, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { AppComponentBase } from "@shared/app-component-base";
import {
  InventoryServiceProxy,
  CreateGoodsReceivedDto,
  CreateGoodsReceivedProductDto,
  CommonKeyValuePairDto,
} from "@shared/service-proxies/service-proxies";
import { forEach as _forEach, map as _map } from "lodash-es";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { MatTableDataSource } from "@angular/material/table";

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
  displayedColumns: string[] = [
    "product-name",
    "quantity",
    "free-quantity",
    "unit-price",
    "selling-price",
    "mrp-price",
    "discount-rate",
    "discount-amount",
    "line-amount",
  ];

  LINE_LEVEL_DATA: CreateGoodsReceivedProductDto[] = [];
  dataSource = new MatTableDataSource<CreateGoodsReceivedProductDto>(
    this.LINE_LEVEL_DATA
  );

  constructor(
    injector: Injector,
    private _inventoryService: InventoryServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {}

  selectedSupplier($event: CommonKeyValuePairDto) {
    console.log($event);
  }

  selectedProducts($event: CommonKeyValuePairDto) {
    console.log($event);

    let l = new CreateGoodsReceivedProductDto();
    l.goodsRecievedId = null;
    l.goodsRecievedNumber = null;
    l.sequenceNumber = this.dataSource.data.length + 1;
    l.productId = $event.id;
    l.productCode = $event.code;
    l.productName = $event.name;
    l.expiryDate = null;
    l.batchNumber = null;
    l.quantity = 0;
    l.freeQuantity = 0;
    l.costPrice = 0;
    l.sellingPrice = 0;
    l.maximumRetailPrice = 0;
    l.discountRate = 0;
    l.discountAmount = 0;
    l.lineTotal = 0;
    this.dataSource.data.push(l);

    console.log(this.dataSource);

    return (this.dataSource.filter = "");
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
