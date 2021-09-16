import { Component, OnInit, Injector } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import {
  ProductionServiceProxy,
  CommonKeyValuePairDto,
  DocumentSequenceNumberManagerImplementationServiceProxy,
  PurchasingServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { forEach as _forEach, map as _map } from "lodash-es";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { MatTableDataSource } from "@angular/material/table";
import {
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
    "order-quantity",
    "free-quantity",
    "cost-price",
    "discount-rate",
    "discount-amount",
    "line-total",
    "actions",
  ];
  dataSource = new MatTableDataSource<FormGroup>();

  poForm;

  validationMessages = {
    purchaseOrderNumber: {
      required: "This field is required",
      maxlength: "Purchase order number should contain maximum 15 characters",
    },
    referenceNumber: {
      maxlength: "Reference number should contain maximum 10 characters",
    },
    remarks: {
      maxlength: "Remark should contain maximum 100 characters",
    },
    supplierId: {
      required: "This field is required",
    },
    taxRate: {
      required: "This field is required",
      min: "Tax rate should contain minimum 0",
      max: "Tax rate should contain maximum 100",
    },
    discountRate: {
      required: "This field is required",
      min: "Discount rate should contain minimum 0",
      max: "Discount rate should contain maximum 100",
    },
  };

  formErrors = {
    purchaseOrderNumber: "",
    referenceNumber: "",
    remarks: "",
    supplierId: "",
    taxRate: "",
    discountRate: "",
  };

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

    this.poForm = this.fb.group({
      purchaseOrderNumber: [
        null,
        Validators.compose([Validators.required, Validators.maxLength(15)]),
      ],
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

    this.poForm.valueChanges.subscribe((data) => {
      this.logValidationErrors(this.poForm);
    });
  }

  logValidationErrors(group: FormGroup = this.poForm): void {
    // Loop through each control key in the FormGroup
    Object.keys(group.controls).forEach((key: string) => {
      // Get the control. The control can be a nested form group
      const abstractControl = group.get(key);
      // If the control is nested form group, recursively call
      // this same method
      if (abstractControl instanceof FormGroup) {
        this.logValidationErrors(abstractControl);
        // If the control is a FormControl
      } else {
        // Clear the existing validation errors
        this.formErrors[key] = "";
        if (
          abstractControl &&
          !abstractControl.valid &&
          (abstractControl.touched || abstractControl.dirty)
        ) {
          // Get all the validation messages of the form control
          // that has failed the validation
          const messages = this.validationMessages[key];
          // Find which validation has failed. For example required,
          // minlength or maxlength. Store that error message in the
          // formErrors object. The UI will bind to this object to
          // display the validation errors
          for (const errorKey in abstractControl.errors) {
            if (errorKey) {
              this.formErrors[key] += messages[errorKey] + " ";
            }
          }
        }
      }
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

  save() {
    this.saving = true;
    if (this.purchaseOrderProducts.length <= 0) {
      this.notify.error(this.l("SelectAtleastOneProduct"));
      this.saving = false;
      return;
    }

    if (this.poForm.valid) {
      this._purchasingService
        .createPurchaseOrder(this.poForm.value)
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
