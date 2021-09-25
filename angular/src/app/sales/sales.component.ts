import { Component, Injector, OnInit } from "@angular/core";
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from "@angular/forms";
import { MatTableDataSource } from "@angular/material/table";
import { AppComponentBase } from "@shared/app-component-base";
import { ProductSearchResultDto } from "@shared/service-proxies/service-proxies";

@Component({
  selector: "app-sales",
  templateUrl: "./sales.component.html",
  styleUrls: ["./sales.component.scss"],
})
export class SalesComponent extends AppComponentBase implements OnInit {
  selectedSearchProductType: number = 1; // product search type
  salePanelForm;
  displayedColumns: string[] = [
    "product-name",
    "sold-price",
    "quantity",
    "free-quantity",
    "discount",
    "line-amount",
    "actions",
  ];
  dataSource = new MatTableDataSource<FormGroup>();

  validationMessages = {
    goodsReceivedNumber: {
      required: "This field is required",
      maxlength: "goods received number should contain maximum 15 characters",
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
    goodsReceivedNumber: "",
    referenceNumber: "",
    remarks: "",
    supplierId: "",
    taxRate: "",
    discountRate: "",
  };

  constructor(injector: Injector, private fb: FormBuilder) {
    super(injector);
  }

  ngOnInit(): void {
    this.salePanelForm = this.fb.group({
      salesNumber: [
        null,
        Validators.compose([Validators.required, Validators.maxLength(15)]),
      ],
      referenceNumber: [null, Validators.maxLength(10)],
      customerId: [null],
      customerCode: [null],
      customerName: [null],
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
      comments: [null, Validators.maxLength(100)],
      salesProducts: this.fb.array([]),
    });

    this.salePanelForm.valueChanges.subscribe((data) => {
      this.logValidationErrors(this.salePanelForm);
    });
  }

  logValidationErrors(group: FormGroup = this.salePanelForm): void {
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

  isProductExist(productId): boolean {
    let product = this.salesProducts.controls.find(
      (p) => p.get("productId").value == productId
    );
    if (!product) {
      return false;
    }
    this.notify.info(this.l("ProductIsAlreadyExist"), "OOPS!");
    return true;
  }

  selectedProducts($event: ProductSearchResultDto) {
    console.log($event);
    if (!this.isProductExist($event.id)) {
      let item = this.fb.group({
        productId: [$event.id, Validators.required],
        productCode: [$event.code, Validators.required],
        productName: [$event.name, Validators.required],
        warehouseId: [null, Validators.required],
        warehouseCode: [null, Validators.required],
        warehouseName: [null, Validators.required],
        quantity: [
          0,
          Validators.compose([Validators.required, Validators.min(1)]),
        ],
        freeQuantity: [
          0,
          Validators.compose([Validators.required, Validators.min(0)]),
        ],
        soldPrice: [
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

      this.salesProducts.push(item);
      this.dataSource.data.push(item);

      return (this.dataSource.filter = "");
    }
  }

  removeProduct(itemIndex: number, item: FormGroup) {
    console.log(this.salePanelForm);
    console.log(this.dataSource);
    this.salesProducts.removeAt(itemIndex);
    this.dataSource.data.splice(itemIndex, 1);
    this.dataSource._updateChangeSubscription();
  }

  updateLineLevelCalculations(item: FormGroup) {
    let __soldPrice = !item.get("soldPrice").value
      ? 0
      : parseFloat(item.get("soldPrice").value);

    let __quantity = !item.get("quantity").value
      ? 0
      : parseFloat(item.get("quantity").value);

    let __discountRate = !item.get("discountRate").value
      ? 0
      : parseFloat(item.get("discountRate").value);

    let _lineTotal = __quantity * __soldPrice;
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
    this.salesProducts.controls.forEach(function (item) {
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
  get salesNumber() {
    return this.salePanelForm.get("salesNumber") as FormControl;
  }

  get referenceNumber() {
    return this.salePanelForm.get("referenceNumber") as FormControl;
  }

  get comments() {
    return this.salePanelForm.get("comments") as FormControl;
  }

  get customerId() {
    return this.salePanelForm.get("customerId") as FormControl;
  }

  get customerCode() {
    return this.salePanelForm.get("customerCode") as FormControl;
  }

  get customerName() {
    return this.salePanelForm.get("customerName") as FormControl;
  }

  get discountRate() {
    return this.salePanelForm.get("discountRate") as FormControl;
  }

  get discountAmount() {
    return this.salePanelForm.get("discountAmount") as FormControl;
  }

  get taxRate() {
    return this.salePanelForm.get("taxRate") as FormControl;
  }

  get taxAmount() {
    return this.salePanelForm.get("taxAmount") as FormControl;
  }

  get grossAmount() {
    return this.salePanelForm.get("grossAmount") as FormControl;
  }

  get netAmount() {
    return this.salePanelForm.get("netAmount") as FormControl;
  }

  get salesProducts(): FormArray {
    return this.salePanelForm.controls["salesProducts"] as FormArray;
  }
  //#endregion
}
