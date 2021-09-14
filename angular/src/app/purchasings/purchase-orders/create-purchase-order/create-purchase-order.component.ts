import { Component, OnInit, Injector } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import {
  InventoryServiceProxy,
  ProductionServiceProxy,
  CommonKeyValuePairDto,
  DocumentSequenceNumberManagerImplementationServiceProxy,
  PurchasingServiceProxy,
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

@Component({
  selector: "app-create-purchase-order",
  templateUrl: "./create-purchase-order.component.html",
  styleUrls: ["./create-purchase-order.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreatePurchaseOrderComponent
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
    "discount-rate",
    "discount-amount",
    "line-amount",
    "actions",
  ];
  dataSource = new MatTableDataSource<FormGroup>();

  poForm = this.fb.group({
    purchaseOrderNumber: [null, Validators.required],
    referenceNumber: [null, Validators.maxLength(10)],
    expectedDeliveryDate: [null],
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
    status: [1],
    remarks: [null, Validators.maxLength(100)],
    purchaseOrderProducts: this.fb.array([]),
  });

  constructor(
    private fb: FormBuilder,
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    private _purchasingService: PurchasingServiceProxy,
    private _documentService: DocumentSequenceNumberManagerImplementationServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllWarehouses();
    this._documentService.getNextDocumentNumber(1).subscribe((result) => {
      this.purchaseOrderNumber.setValue(result);
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
    let product = this.purchaseOrderProducts.controls.find(
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
        sequenceNo: [0, Validators.required],
        productId: [$event.id, Validators.required],
        productCode: [$event.code, Validators.required],
        productName: [$event.name, Validators.required],
        warehouseId: [null, Validators.required],
        warehouseCode: [null, Validators.required],
        warehouseName: [null, Validators.required],
        orderQuantity: [
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
        remarks: [null, Validators.maxLength(100)],
      });

      this.purchaseOrderProducts.push(item);
      this.dataSource.data.push(item);

      return (this.dataSource.filter = "");
    }
  }

  removeProduct(itemIndex: number, item: FormGroup) {
    this.purchaseOrderProducts.removeAt(itemIndex);
    this.dataSource.data.splice(itemIndex, 1);
    this.dataSource._updateChangeSubscription();
  }

  updateLineLevelCalculations(item: FormGroup) {
    let __costPrice = !item.get("costPrice").value
      ? 0
      : parseFloat(item.get("costPrice").value);

    let __orderQuantity = !item.get("orderQuantity").value
      ? 0
      : parseFloat(item.get("orderQuantity").value);

    let __discountRate = !item.get("discountRate").value
      ? 0
      : parseFloat(item.get("discountRate").value);

    let _lineTotal = __orderQuantity * __costPrice;
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
    this.purchaseOrderProducts.controls.forEach(function (item) {
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

  save() {}

  //#region Propertises
  get purchaseOrderNumber() {
    return this.poForm.get("purchaseOrderNumber") as FormControl;
  }

  get referenceNumber() {
    return this.poForm.get("referenceNumber") as FormControl;
  }

  get expectedDeliveryDate() {
    return this.poForm.get("expectedDeliveryDate") as FormControl;
  }

  get supplierId() {
    return this.poForm.get("supplierId") as FormControl;
  }

  get supplierCode() {
    return this.poForm.get("supplierCode") as FormControl;
  }

  get supplierName() {
    return this.poForm.get("supplierName") as FormControl;
  }

  get discountRate() {
    return this.poForm.get("discountRate") as FormControl;
  }

  get discountAmount() {
    return this.poForm.get("discountAmount") as FormControl;
  }

  get taxRate() {
    return this.poForm.get("taxRate") as FormControl;
  }

  get taxAmount() {
    return this.poForm.get("taxAmount") as FormControl;
  }

  get grossAmount() {
    return this.poForm.get("grossAmount") as FormControl;
  }

  get netAmount() {
    return this.poForm.get("netAmount") as FormControl;
  }

  get remarks() {
    return this.poForm.get("remarks") as FormControl;
  }

  get purchaseOrderProducts(): FormArray {
    return this.poForm.controls["purchaseOrderProducts"] as FormArray;
  }
  //#endregion
}
