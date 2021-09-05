import { Component, OnInit, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { AppComponentBase } from "@shared/app-component-base";
import {
  InventoryServiceProxy,
  ProductionServiceProxy,
  CreateGoodsReceivedDto,
  CreateGoodsReceivedProductDto,
  CommonKeyValuePairDto,
} from "@shared/service-proxies/service-proxies";
import { forEach as _forEach, map as _map } from "lodash-es";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { MatTableDataSource } from "@angular/material/table";
import { Observable } from "rxjs";
import { FormControl, Validators } from "@angular/forms";

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
  warehouses: CommonKeyValuePairDto[];
  grn = new CreateGoodsReceivedDto();
  displayedColumns: string[] = [
    "product-name",
    "warehouse",
    "quantity",
    "free-quantity",
    "cost-price",
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
    private _productionService: ProductionServiceProxy,
    private _inventoryService: InventoryServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllWarehouses();
    console.log(this.warehouses);
  }

  getAllWarehouses() {
    this._productionService
      .getAllKeyValuePairWarehouses()
      .subscribe((result) => (this.warehouses = result));
  }

  selectWarehouse($event, lineLevelDto: CreateGoodsReceivedProductDto) {
    let warehouseId = $event.target.value;
    let objWarehouse = new CommonKeyValuePairDto();

    if (warehouseId) {
      objWarehouse = this.warehouses.find((x) => x.id == warehouseId);
    }

    // set value for warehouse
    lineLevelDto.warehouseId = !objWarehouse.id ? null : objWarehouse.id;
    lineLevelDto.warehouseCode = !objWarehouse.id ? null : objWarehouse.code;
    lineLevelDto.warehouseName = !objWarehouse.id ? null : objWarehouse.name;

    console.log(lineLevelDto);
  }

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
    l.warehouseId = null;
    l.warehouseCode = null;
    l.warehouseName = null;
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
