import { Component, Injector, OnInit } from "@angular/core";
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { MatTableDataSource } from "@angular/material/table";
import { ActivatedRoute, Router } from "@angular/router";
import { AppComponentBase } from "@shared/app-component-base";
import {
  CreateOrUpdateTempSalesHeaderDto,
  CreateTempSalesProductDto,
  TempSalesHeaderDto,
  ProductSearchResultDto,
  ProductStockBalanceDto,
  SalesServiceProxy,
  TempSalesProductDto,
} from "@shared/service-proxies/service-proxies";
import { StockBalanceDialogComponent } from "./stock-balance/stock-balance-dialog.component";

@Component({
  selector: "app-sales",
  templateUrl: "./sales.component.html",
  styleUrls: ["./sales.component.scss"],
})
export class SalesComponent extends AppComponentBase implements OnInit {
  tempSalesHeaderId: number = 0;
  selectedSearchProductType: number = 1; // product search type
  salePanelForm;
  displayedColumns: string[] = [
    "product-name",
    "sold-price",
    "quantity",
    "discount",
    "line-amount",
    "actions",
  ];
  dataSource = new MatTableDataSource<FormGroup>();
  productStockBalances: ProductStockBalanceDto[] = [];
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
  tempProducts;

  constructor(
    injector: Injector,
    private fb: FormBuilder,
    private _matDialogService: MatDialog,
    private _salesService: SalesServiceProxy,
    private router: Router,
    private route: ActivatedRoute
  ) {
    super(injector);
  }

  ngOnInit(): void {
    let _tempSalesHeaderId = (this.tempSalesHeaderId =
      +this.route.snapshot.queryParamMap.get("salesHeaderId"));
    if (!_tempSalesHeaderId) this.populateSalesHeaderDetails(true, null);
    if (_tempSalesHeaderId) this.getTemporarySalesDetails(_tempSalesHeaderId);
  }

  getTemporarySalesDetails(id: number) {
    this._salesService
      .getTempSales(id)
      .subscribe((result: TempSalesHeaderDto) => {
        console.log(result);

        this.populateSalesHeaderDetails(false, result);

        let stockBalanceIds: string[] = [];
        result.tempSalesProducts.forEach((value) => {
          stockBalanceIds.push(value.stockBalanceId);
        });

        let _stockBalances: ProductStockBalanceDto[] = [];
        if (stockBalanceIds.length > 0) {
          this._salesService
            .getStockBalancesByStockBalanceIds(stockBalanceIds)
            .subscribe((sb) => {
              _stockBalances = sb;

              result.tempSalesProducts.forEach((value, i) => {
                let particularSB = _stockBalances.find(
                  (sb) => sb.stockBalanceId == value.stockBalanceId
                );
                this.populateSalesProductDetails(
                  false,
                  value,
                  null,
                  particularSB
                );
              });

              this.notify.info(this.l("ProductRetreivedSuccessfully"));
            });
        }
      });
  }

  // If "salesheader: Exist => Came from Query string else a new One
  private populateSalesHeaderDetails(
    isNewSale: boolean,
    salesheader: TempSalesHeaderDto
  ) {
    this.salePanelForm = this.fb.group({
      salesNumber: [
        null,
        Validators.compose([Validators.required, Validators.maxLength(15)]),
      ],
      referenceNumber: [null, Validators.maxLength(10)],
      customerId: [isNewSale ? null : salesheader.customerId],
      customerCode: [isNewSale ? null : salesheader.customerCode],
      customerName: [isNewSale ? null : salesheader.customerName],
      discountRate: [
        isNewSale ? 0 : salesheader.discountRate,
        Validators.compose([
          Validators.required,
          Validators.min(0),
          Validators.max(100),
        ]),
      ],
      discountAmount: [
        isNewSale ? 0 : salesheader.discountAmount,
        Validators.required,
      ],
      taxRate: [
        isNewSale ? 0 : salesheader.taxRate,
        Validators.compose([
          Validators.required,
          Validators.min(0),
          Validators.max(100),
        ]),
      ],
      taxAmount: [isNewSale ? 0 : salesheader.taxAmount, Validators.required],
      grossAmount: [
        isNewSale ? 0 : salesheader.grossAmount,
        Validators.required,
      ],
      netAmount: [isNewSale ? 0 : salesheader.netAmount, Validators.required],
      comments: [
        isNewSale ? null : salesheader.remarks,
        Validators.maxLength(100),
      ],
      salesProducts: this.fb.array([]),
    });

    console.log(this.salePanelForm);

    this.salePanelForm.valueChanges.subscribe((data) => {
      this.logValidationErrors(this.salePanelForm);
    });
  }

  private populateSalesProductDetails(
    isNewSale: boolean,
    _tempSalesProduct: TempSalesProductDto,
    _product: ProductSearchResultDto,
    _stockBalance: ProductStockBalanceDto
  ) {
    let item = this.fb.group({
      stockBalanceId: [_stockBalance.stockBalanceId],
      productId: [
        !isNewSale ? _tempSalesProduct.productId : _product.id,
        Validators.required,
      ],
      productCode: [
        !isNewSale ? _tempSalesProduct.code : _product.code,
        Validators.required,
      ],
      productName: [
        !isNewSale ? _tempSalesProduct.name : _product.name,
        Validators.required,
      ],
      warehouseId: [
        !isNewSale ? _tempSalesProduct.warehouseId : null,
        Validators.required,
      ],
      warehouseCode: [
        !isNewSale ? _tempSalesProduct.warehouseCode : null,
        Validators.required,
      ],
      warehouseName: [
        !isNewSale ? _tempSalesProduct.warehouseName : null,
        Validators.required,
      ],
      quantity: [
        !isNewSale ? _tempSalesProduct.quantity : 0,
        Validators.compose([
          Validators.required,
          Validators.min(1),
          Validators.max(
            !isNewSale
              ? _tempSalesProduct.bookBalanceQuantity
              : _stockBalance.bookBalanceQuantity
          ),
        ]),
      ],
      freeQuantity: [
        0,
        Validators.compose([Validators.required, Validators.min(0)]),
      ],
      soldPrice: [
        !isNewSale
          ? _tempSalesProduct.sellingPrice
          : _stockBalance.sellingPrice,
        Validators.compose([Validators.required, Validators.min(1)]),
      ],
      discountRate: [
        !isNewSale ? _tempSalesProduct.discountRate : 0,
        Validators.compose([
          Validators.required,
          Validators.min(0),
          Validators.max(100),
        ]),
      ],
      discountAmount: [
        !isNewSale ? _tempSalesProduct.discountAmount : 0,
        Validators.required,
      ],
      lineTotal: [
        !isNewSale ? _tempSalesProduct.lineTotal : 0,
        Validators.required,
      ],
      stockBalance: _stockBalance, // TODO: No needed I guess
    });
    this.salesProducts.push(item);
    this.dataSource.data.push(item);
    return (this.dataSource.filter = "");
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

  isStockBalanceExist(stockBalanceId): boolean {
    let product = this.salesProducts.controls.find(
      (p) => p.get("stockBalanceId").value == stockBalanceId
    );
    if (!product) {
      return false;
    }
    this.notify.info(this.l("ProductIsAlreadyExist"), "OOPS!");
    return true;
  }

  showStockBalanceDialog(product: ProductSearchResultDto) {
    let materialDialog = this._matDialogService.open(
      StockBalanceDialogComponent,
      {
        data: { id: product.id, productName: product.name },
        width: "70%",
      }
    );

    materialDialog.afterClosed().subscribe((result) => {
      // "SelectedProduct" came from Dialog
      if (result && result.event == "SelectedProduct") {
        this.addProductToTable(product, result.data);
      }
    });
  }

  selectedProducts($event: ProductSearchResultDto) {
    this.showStockBalanceDialog($event);
  }

  addProductToTable(
    product: ProductSearchResultDto,
    stockBalances: ProductStockBalanceDto[]
  ) {
    for (let i = 0; i < stockBalances.length; i++) {
      const sb = stockBalances[i];

      if (!this.isStockBalanceExist(sb.stockBalanceId)) {
        this.populateSalesProductDetails(true, null, product, sb);
      }
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

  newSale() {
    this.router.navigate(["/app/sales"]);
  }

  payment() {
    // 1. Create a Temp Sales Header & Product
    // 2. Get the Id when creating it
    // 3. From the returned Id navigate to payment page
    if (this.salePanelForm.value.salesProducts.length <= 0) {
      this.notify.info(this.l("SelectAtleasetOneProduct"));
      return;
    }

    console.log(this.salePanelForm.value);

    let _header = new CreateOrUpdateTempSalesHeaderDto();
    _header.id = !this.tempSalesHeaderId ? null : this.tempSalesHeaderId;
    _header.customerId = this.salePanelForm.value.customerId;
    _header.customerCode = this.salePanelForm.value.customerCode;
    _header.customerName = this.salePanelForm.value.customerName;
    _header.discountRate = this.salePanelForm.value.discountRate;
    _header.discountAmount = this.salePanelForm.value.discountAmount;
    _header.taxRate = this.salePanelForm.value.taxRate;
    _header.taxAmount = this.salePanelForm.value.taxAmount;
    _header.grossAmount = this.salePanelForm.value.grossAmount;
    _header.netAmount = this.salePanelForm.value.netAmount;
    _header.remarks = this.salePanelForm.value.comments;
    _header.isActive = true;
    _header.tempSalesProducts = [];

    this.salePanelForm.value.salesProducts.forEach((item, index) => {
      let _lineLevel = new CreateTempSalesProductDto();
      _lineLevel.productId = item.productId;
      // y.barCode = item.; // missing in item.
      _lineLevel.code = item.productCode;
      _lineLevel.name = item.productName;
      _lineLevel.stockBalanceId = item.stockBalance.stockBalanceId;
      _lineLevel.expiryDate = item.stockBalance.expiryDate;
      _lineLevel.batchNumber = item.stockBalance.batchNumber;
      _lineLevel.warehouseId = null;
      _lineLevel.warehouseCode = null;
      _lineLevel.warehouseName = null;
      _lineLevel.bookBalanceQuantity = item.stockBalance.bookBalanceQuantity;
      _lineLevel.bookBalanceUnitOfMeasureUnit =
        item.stockBalance.bookBalanceUnitOfMeasureUnit;
      _lineLevel.costPrice = item.stockBalance.costPrice;
      _lineLevel.sellingPrice = item.stockBalance.sellingPrice;
      _lineLevel.maximumRetailPrice = item.stockBalance.maximumRetailPrice;
      _lineLevel.isSelected = item.stockBalance.isSelected;
      _lineLevel.discountRate = item.discountRate;
      _lineLevel.discountAmount = item.discountAmount;
      _lineLevel.quantity = item.quantity;
      _lineLevel.lineTotal = item.lineTotal;
      _lineLevel.isActive = true;

      _header.tempSalesProducts.push(_lineLevel);
    });

    console.log(_header);
    this._salesService.createOrUpdateTempSales(_header).subscribe((i) => {
      console.log(i.id);
      this.notify.info(this.l("SavedSuccessfully"));
      this.router.navigate(["/app/payment-component", i.id]);
    });
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
