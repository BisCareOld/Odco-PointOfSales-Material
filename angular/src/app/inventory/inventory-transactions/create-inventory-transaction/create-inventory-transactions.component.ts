import { Component, OnInit, Injector } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import {
  InventoryServiceProxy,
  ProductionServiceProxy,
  CommonKeyValuePairDto,
  DocumentSequenceNumberManagerImplementationServiceProxy,
  CreateGoodsReceivedDto,
  CreateGoodsReceivedProductDto,
} from "@shared/service-proxies/service-proxies";
import { forEach as _forEach, map as _map } from "lodash-es";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { MatTableDataSource } from "@angular/material/table";
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from "@angular/forms";
import { finalize } from "rxjs/operators";
import { ThrowStmt } from "@angular/compiler";

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
    "actions",
  ];
  dataSource = new MatTableDataSource<FormGroup>();

  grnForm = this.fb.group({
    goodsReceivedNumber: [null, Validators.required],
    referenceNumber: [null, Validators.maxLength(10)],
    supplierId: [null, Validators.required],
    supplierCode: [null, Validators.required],
    supplierName: [null, Validators.required],
    discountRate: [
      0,
      Validators.compose([
        Validators.required,
        Validators.min(0),
        Validators.max(100),
      ]),
    ],
    discountAmount: [0, Validators.required],
    taxRate: [
      0,
      Validators.compose([
        Validators.required,
        Validators.min(0),
        Validators.max(100),
      ]),
    ],
    taxAmount: [0, Validators.required],
    grossAmount: [0, Validators.required],
    netAmount: [0, Validators.required],
    transactionStatus: [1],
    remarks: [null],
    goodsReceivedProducts: this.fb.array([]),
  });

  constructor(
    private fb: FormBuilder,
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
      this.goodsReceivedNumber.setValue(result);
    });
  }

  getAllWarehouses() {
    this._productionService
      .getAllKeyValuePairWarehouses()
      .subscribe((result) => (this.warehouses = result));
  }

  selectWarehouse($event, item: FormGroup) {
    let warehouseId = $event.target.value;
    let objWarehouse = new CommonKeyValuePairDto();

    if (warehouseId) {
      objWarehouse = this.warehouses.find((x) => x.id == warehouseId);
    }

    // set value for warehouse
    item.get("warehouseId").setValue(!objWarehouse.id ? null : objWarehouse.id);
    item
      .get("warehouseCode")
      .setValue(!objWarehouse.id ? null : objWarehouse.code);
    item
      .get("warehouseName")
      .setValue(!objWarehouse.id ? null : objWarehouse.name);
  }

  selectedSupplier($event: CommonKeyValuePairDto) {
    this.supplierId.setValue(!$event.id ? null : $event.id);
    this.supplierCode.setValue(!$event.code ? null : $event.code);
    this.supplierName.setValue(!$event.name ? null : $event.name);
  }

  isProductExist(productId): boolean {
    let product = this.goodsReceivedProducts.controls.find(
      (p) => p.get("productId").value == productId
    );
    if (!product) {
      return false;
    }
    this.notify.info(this.l("ProductIsAlreadyExist"));
    return true;
  }

  selectedProducts($event: CommonKeyValuePairDto) {
    if (!this.isProductExist($event.id)) {
      let item = this.fb.group({
        goodsRecievedNumber: [
          this.goodsReceivedNumber.value,
          Validators.required,
        ],
        sequenceNumber: [0, Validators.required],
        productId: [$event.id, Validators.required],
        productCode: [$event.code, Validators.required],
        productName: [$event.name, Validators.required],
        warehouseId: [null, Validators.required],
        warehouseCode: [null, Validators.required],
        warehouseName: [null, Validators.required],
        expiryDate: [null],
        batchNumber: [null],
        quantity: [
          0,
          Validators.compose([Validators.required, Validators.min(1)]),
        ],
        freeQuantity: [
          0,
          Validators.compose([Validators.required, Validators.min(0)]),
        ],
        costPrice: [
          0,
          Validators.compose([Validators.required, Validators.min(1)]),
        ],
        sellingPrice: [
          0,
          Validators.compose([Validators.required, Validators.min(1)]),
        ],
        maximumRetailPrice: [
          0,
          Validators.compose([Validators.required, Validators.min(1)]),
        ],
        discountRate: [
          0,
          Validators.compose([
            Validators.required,
            Validators.min(0),
            Validators.max(100),
          ]),
        ],
        discountAmount: [0, Validators.required],
        lineTotal: [0, Validators.required],
      });

      this.goodsReceivedProducts.push(item);
      this.dataSource.data.push(item);

      return (this.dataSource.filter = "");
    }
  }

  removeProduct(itemIndex: number, item: FormGroup) {
    console.log(this.grnForm);
    console.log(this.dataSource);
    this.goodsReceivedProducts.removeAt(itemIndex);
    this.dataSource.data.splice(itemIndex, 1);
    this.dataSource._updateChangeSubscription();
  }

  updateLineLevelCalculations(item: FormGroup) {
    let __costPrice = !item.get("costPrice").value
      ? 0
      : parseFloat(item.get("costPrice").value);

    let __quantity = !item.get("quantity").value
      ? 0
      : parseFloat(item.get("quantity").value);

    let __discountRate = !item.get("discountRate").value
      ? 0
      : parseFloat(item.get("discountRate").value);

    let _lineTotal = __quantity * __costPrice;
    let _discountAmount = parseFloat(
      ((_lineTotal * __discountRate) / 100).toFixed(2)
    );
    item.get("discountRate").setValue(__discountRate);
    item.get("discountAmount").setValue(_discountAmount);
    item
      .get("lineTotal")
      .setValue(parseFloat((_lineTotal - _discountAmount).toFixed(2)));
    this.headerLevelCalculation();
    console.log(item);
  }

  calculateLineLevelTotal() {
    let total = 0;
    this.goodsReceivedProducts.controls.forEach(function (item) {
      total += item.get("lineTotal").value;
    });
    this.grossAmount.setValue(parseFloat(total.toFixed(2)));
    return total.toFixed(2);
  }

  headerLevelCalculation() {
    // (gross value + tax – discount)
    let grossTotal = parseFloat(this.calculateLineLevelTotal());
    let tax = parseFloat((grossTotal * (this.taxRate.value / 100)).toFixed(2));
    let discount = parseFloat(
      (grossTotal * (this.discountRate.value / 100)).toFixed(2)
    );
    this.discountAmount.setValue(discount);
    this.taxAmount.setValue(tax);
    let netAmount = parseFloat((grossTotal + tax - discount).toFixed(2));
    this.netAmount.setValue(netAmount);
  }

  validateForm() {
    // this.errors = [];
    // if (!this.grn.supplierId) {
    //   this.errors.push(this.l("SupplierIsRequired!"));
    // }
    // if (this.LINE_LEVEL_DATA.length == 0) {
    //   this.errors.push(this.l("SelectAtleastOneProduct!"));
    // }
  }

  save() {
    this.saving = true;
    if (this.goodsReceivedProducts.length <= 0) {
      this.notify.error(this.l("SelectAtleastOneProduct"));
      this.saving = false;
      return;
    }

    if (this.grnForm.valid) {
      this._inventoryService
        .createGoodsReceivedNote(this.grnForm.value)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        )
        .subscribe(() => {
          this.notify.info(this.l("SavedSuccessfully"));
        });
    } else {
      this.notify.error(this.l("FormIsNotValid"));
    }
  }

  //#region Propertises
  get goodsReceivedNumber() {
    return this.grnForm.get("goodsReceivedNumber") as FormControl;
  }

  get referenceNumber() {
    return this.grnForm.get("referenceNumber") as FormControl;
  }

  get remarks() {
    return this.grnForm.get("remarks") as FormControl;
  }

  get supplierId() {
    return this.grnForm.get("supplierId") as FormControl;
  }

  get supplierCode() {
    return this.grnForm.get("supplierCode") as FormControl;
  }

  get supplierName() {
    return this.grnForm.get("supplierName") as FormControl;
  }

  get discountRate() {
    return this.grnForm.get("discountRate") as FormControl;
  }

  get discountAmount() {
    return this.grnForm.get("discountAmount") as FormControl;
  }

  get taxRate() {
    return this.grnForm.get("taxRate") as FormControl;
  }

  get taxAmount() {
    return this.grnForm.get("taxAmount") as FormControl;
  }

  get grossAmount() {
    return this.grnForm.get("grossAmount") as FormControl;
  }

  get netAmount() {
    return this.grnForm.get("netAmount") as FormControl;
  }

  get goodsReceivedProducts(): FormArray {
    return this.grnForm.controls["goodsReceivedProducts"] as FormArray;
  }
  //#endregion
}
