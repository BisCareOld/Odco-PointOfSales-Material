import { Component, OnInit, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { AppComponentBase } from "@shared/app-component-base";
import {
  InventoryServiceProxy,
  ProductionServiceProxy,
  CreateGoodsReceivedDto,
  CreateGoodsReceivedProductDto,
  CommonKeyValuePairDto,
  DocumentSequenceNumberManagerImplementationServiceProxy,
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
  errors: string[] = [];

  constructor(
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    private _inventoryService: InventoryServiceProxy,
    private _documentService: DocumentSequenceNumberManagerImplementationServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllWarehouses();
    this._documentService.getNextDocumentNumber(2).subscribe((result) => {
      this.grn.goodsReceivedNumber = result;
    });
    this.grn.grossAmount = 0.0;
    this.grn.taxRate = 0.0;
    this.grn.taxAmount = 0.0;
    this.grn.discountRate = 0.0;
    this.grn.discountAmount = 0.0;
    this.grn.netAmount = 0.0;
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
  }

  selectedSupplier($event: CommonKeyValuePairDto) {
    this.grn.supplierId = !$event.id ? null : $event.id;
    this.grn.supplierCode = !$event.code ? null : $event.code;
    this.grn.supplierName = !$event.name ? null : $event.name;
  }

  isProductExist(productId): boolean {
    var lineLevelProduct = this.LINE_LEVEL_DATA.find(
      (p) => p.productId == productId
    );

    if (!lineLevelProduct) {
      return false;
    }
    this.notify.info(this.l("ProductIsAlreadyExist"));
    return true;
  }

  selectedProducts($event: CommonKeyValuePairDto) {
    if (!this.isProductExist($event.id)) {
      let l = new CreateGoodsReceivedProductDto();
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
      return (this.dataSource.filter = "");
    }
  }

  updateLineLevelCalculations(grp: CreateGoodsReceivedProductDto) {
    let _quantity = grp.quantity;

    let _costPrice = parseFloat(grp.costPrice.toFixed(2));
    let _lineTotal = _quantity * _costPrice;
    let _discountRate = parseFloat(grp.discountRate.toFixed(2));
    let _discountAmount = parseFloat(
      ((_lineTotal * _discountRate) / 100).toFixed(2)
    );

    grp.discountRate = _discountRate;
    grp.discountAmount = _discountAmount;
    grp.lineTotal = parseFloat((_lineTotal - _discountAmount).toFixed(2));

    this.headerLevelCalculation();
  }

  calculateLineLevelTotal() {
    let total = 0;
    this.LINE_LEVEL_DATA.forEach(function (item) {
      total += item.lineTotal;
    });
    this.grn.grossAmount = parseFloat(total.toFixed(2));
    return total.toFixed(2);
  }

  headerLevelCalculation() {
    // (gross value + tax – discount)
    let grossTotal = parseFloat(this.calculateLineLevelTotal());

    let tax = parseFloat((grossTotal * (this.grn.taxRate / 100)).toFixed(2));

    let discount = parseFloat(
      (grossTotal * (this.grn.discountRate / 100)).toFixed(2)
    );

    this.grn.discountAmount = discount;

    this.grn.taxAmount = tax;

    let netAmount = parseFloat((grossTotal + tax - discount).toFixed(2));

    this.grn.netAmount = netAmount;
  }

  validateForm() {
    this.errors = [];

    if (!this.grn.supplierId) {
      this.errors.push(this.l("SupplierIsRequired!"));
    }

    if (this.LINE_LEVEL_DATA.length == 0) {
      this.errors.push(this.l("SelectAtleastOneProduct!"));
    }
  }

  save() {
    this.validateForm();

    if (this.errors.length > 0) {
      return;
    }

    this.grn.goodsReceivedProducts = this.LINE_LEVEL_DATA;

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
