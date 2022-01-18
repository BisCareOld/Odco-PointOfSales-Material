import { Component, Injector, OnInit, ViewChild } from "@angular/core";
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
  CreateOrUpdateSaleDto,
  CreateInventorySalesProductDto,
  SaleDto,
  ProductSearchResultDto,
  ProductStockBalanceDto,
  SalesServiceProxy,
  InventorySalesProductDto,
  NonInventorySalesProductDto,
  CreateNonInventorySalesProductDto,
  CustomerSearchResultDto,
  GroupBySellingPriceDto
} from "@shared/service-proxies/service-proxies";
import { CreateNonInventoryProductDialogComponent } from "../create-non-inventory-product/create-non-inventory-product-dialog.component";
import { StockBalanceDialogComponent } from "../stock-balance/stock-balance-dialog.component";
import { MatAccordion } from '@angular/material/expansion';

@Component({
  selector: 'app-create-sales',
  templateUrl: './create-sales.component.html',
  styleUrls: ['./create-sales.component.scss']
})
export class CreateSalesComponent extends AppComponentBase implements OnInit {
  @ViewChild(MatAccordion) accordion: MatAccordion;
  saleId: string = "";
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
    let _tempSalesHeaderId = (this.saleId = this.route.snapshot.queryParamMap.get("salesHeaderId"));

    if (this.isNullOrEmptyString(_tempSalesHeaderId)) {
      this.populateSalesDetails(true, null);
    }
    else {
      this.getSalesDetails(_tempSalesHeaderId);
    }
  }

  getSalesDetails(id: string) {
    this._salesService
      .getSales(id)
      .subscribe((result: SaleDto) => {

        this.getNonInventorySalesProductsBySaleId(id);
        this.populateSalesDetails(false, result);
        console.log(result);
        result.inventorySalesProducts.forEach((value, i) => {
          this.populateSalesProductDetails(
            false,
            value,
            null,
            null
          );
        });

      });
  }

  getNonInventorySalesProductsBySaleId(id: string) {
    this._salesService.getNonInventoryProductBySaleId(id).subscribe((results) => {
      results.forEach((n: NonInventorySalesProductDto) => {
        this.populateSalesProductForNonInventoryDetails(n);
      });
    });
  }

  // If "salesheader: Exist => Came from Query string else a new One
  private populateSalesDetails(
    isNewSale: boolean,
    salesheader: SaleDto
  ) {
    this.salePanelForm = this.fb.group({
      salesNumber: [
        salesheader != null ? salesheader.salesNumber : null,
        Validators.compose([Validators.required, Validators.maxLength(15)]),
      ],
      referenceNumber: [salesheader != null ? salesheader.referenceNumber : null, Validators.maxLength(15)],
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
      remarks: [
        isNewSale ? null : salesheader.remarks,
        Validators.maxLength(100),
      ],
      isActive: [true],
      salesProducts: this.fb.array([]),
    });

    this.salePanelForm.valueChanges.subscribe((data) => {
      this.logValidationErrors(this.salePanelForm);
    });
  }

  private populateSalesProductDetails(
    isNewSale: boolean,
    _salesProduct: InventorySalesProductDto,   // !isNewSale
    _product: ProductSearchResultDto, // isNewSale
    _groupBySellingPrice: GroupBySellingPriceDto  // isNewSale
  ) {
    let item = this.fb.group({
      id: [!isNewSale ? _salesProduct.id : null],
      salesId: [!isNewSale ? _salesProduct.saleId : null],
      salesNumber: [!isNewSale ? _salesProduct.salesNumber : null],
      productId: [
        !isNewSale ? _salesProduct.productId : _product.id,
        Validators.required,
      ],
      productCode: [
        !isNewSale ? _salesProduct.code : _product.code,
        Validators.required,
      ],
      productName: [
        !isNewSale ? _salesProduct.name : _product.name,
        Validators.required,
      ],
      warehouseId: [
        !isNewSale ? _salesProduct.warehouseId : _groupBySellingPrice.warehouseId,
        Validators.required,
      ],
      warehouseCode: [
        !isNewSale ? _salesProduct.warehouseCode : _groupBySellingPrice.warehouseCode,
        Validators.required,
      ],
      warehouseName: [
        !isNewSale ? _salesProduct.warehouseName : _groupBySellingPrice.warehouseName,
        Validators.required,
      ],
      quantity: [
        !isNewSale ? _salesProduct.quantity : 0,
        Validators.compose([
          Validators.required,
          Validators.min(1),
          Validators.max(
            !isNewSale
              ? _salesProduct.totalBookBalanceQuantity + _salesProduct.receivedQuantity
              : _groupBySellingPrice.totalBookBalanceQuantity
          ),
        ]),
      ],
      freeQuantity: [
        0,
        Validators.compose([Validators.required, Validators.min(0)]),
      ],
      sellingPrice: [!isNewSale
        ? _salesProduct.sellingPrice
        : _groupBySellingPrice.sellingPrice
      ],
      soldPrice: [
        !isNewSale
          ? _salesProduct.price
          : _groupBySellingPrice.sellingPrice,
        Validators.compose([Validators.required, Validators.min(1)]),
      ],
      discountRate: [
        !isNewSale ? _salesProduct.discountRate : 0,
        Validators.compose([
          Validators.required,
          Validators.min(0),
          Validators.max(100),
        ]),
      ],
      discountAmount: [
        !isNewSale ? _salesProduct.discountAmount : 0,
        Validators.required,
      ],
      lineTotal: [
        !isNewSale ? _salesProduct.lineTotal : 0,
        Validators.required,
      ],
      remarks: [null],
      isNonInventoryProductInvolved: [false],
    });
    this.salesProducts.push(item);
    this.dataSource.data.push(item);
    return (this.dataSource.filter = "");
  }

  private populateSalesProductForNonInventoryDetails(
    _nonInventorySalesProduct: NonInventorySalesProductDto,
  ) {
    let item = this.fb.group({
      id: [_nonInventorySalesProduct.id],
      salesId: [_nonInventorySalesProduct.saleId],
      salesNumber: [_nonInventorySalesProduct.salesNumber],
      productId: [
        _nonInventorySalesProduct.productId,
        Validators.required,
      ],
      productCode: [
        _nonInventorySalesProduct.productCode,
        Validators.required,
      ],
      productName: [
        _nonInventorySalesProduct.productName,
        Validators.required,
      ],
      warehouseId: [
        _nonInventorySalesProduct.warehouseId,
        Validators.required,
      ],
      warehouseCode: [
        _nonInventorySalesProduct.warehouseCode,
        Validators.required,
      ],
      warehouseName: [
        _nonInventorySalesProduct.warehouseName,
        Validators.required,
      ],
      quantity: [
        _nonInventorySalesProduct.quantity,
        Validators.compose([
          Validators.required,
          Validators.min(1),
        ]),
      ],
      freeQuantity: [
        0,
        Validators.compose([Validators.required, Validators.min(0)]),
      ],
      sellingPrice: [_nonInventorySalesProduct.sellingPrice],
      soldPrice: [
        _nonInventorySalesProduct.price > 0 ? _nonInventorySalesProduct.price : _nonInventorySalesProduct.sellingPrice,
        Validators.compose([Validators.required, Validators.min(1)]),
      ],
      discountRate: [
        _nonInventorySalesProduct.discountRate,
        Validators.compose([
          Validators.required,
          Validators.min(0),
          Validators.max(100),
        ]),
      ],
      discountAmount: [
        _nonInventorySalesProduct.discountAmount,
        Validators.required,
      ],
      lineTotal: [
        _nonInventorySalesProduct.lineTotal,
        Validators.required,
      ],
      remarks: [null],
      isNonInventoryProductInvolved: [true],
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

  selectCustomer($event: CustomerSearchResultDto) {
    this.customerId.setValue($event.id);
    this.customerCode.setValue($event.code);
    this.customerName.setValue($event.name);
  }

  removeCustomer() {
    this.customerId.setValue(null);
    this.customerCode.setValue(null);
    this.customerName.setValue(null);
  }

  // GroupBy Selling Price
  isProductExistInTable(productId, sellingPrice): boolean {
    let product = this.salesProducts.controls.find(
      (p) => p.get("productId").value == productId && p.get("sellingPrice").value == sellingPrice
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
        width: "65%",
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
    stockBalances: GroupBySellingPriceDto[]
  ) {
    for (let i = 0; i < stockBalances.length; i++) {
      const sb = stockBalances[i];

      if (!this.isProductExistInTable(sb.productId, sb.sellingPrice)) {
        this.populateSalesProductDetails(true, null, product, sb);
      }
    }
  }

  removeProduct(itemIndex: number, item: FormGroup) {
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
    this.router.navigate(["/app/create-sales"]);
  }

  showCreateOrEditNonInventoryProductDialog(): void {
    let materialDialog = this._matDialogService.open(
      CreateNonInventoryProductDialogComponent,
      {
        width: "50%",
      }
    );

    materialDialog.afterClosed().subscribe((result) => {
      // "NonInventoryProduct" came from Dialog
      if (result && result.event == "NonInventoryProduct") {
        this.populateSalesProductForNonInventoryDetails(result.data);
      }
    });
  }

  payment() {
    // 1. Create a Temp Sales Header & Product
    // 2. Get the Id when creating it
    // 3. From the returned Id navigate to payment page
    if (this.salePanelForm.value.salesProducts.length <= 0) {
      this.notify.info(this.l("SelectAtleasetOneProduct"));
      return;
    }

    let _header = new CreateOrUpdateSaleDto();
    _header.id = !this.saleId ? null : this.saleId;
    _header.salesNumber = this.salesNumber.value;
    _header.referenceNumber = null;
    _header.customerId = this.customerId.value;
    _header.customerCode = this.customerCode.value;
    _header.customerName = this.customerName.value;
    _header.discountRate = this.discountRate.value;
    _header.discountAmount = this.discountAmount.value;
    _header.taxRate = this.taxRate.value;
    _header.taxAmount = this.taxAmount.value;
    _header.grossAmount = this.grossAmount.value;
    _header.netAmount = this.netAmount.value;
    _header.remarks = this.comments != null ? this.comments.value : null;
    _header.isActive = true;
    _header.inventorySalesProducts = [];
    _header.nonInventorySalesProducts = [];

    let sequenceNumber = 1;

    this.salePanelForm.value.salesProducts.forEach((item, index) => {

      let _a = new CreateInventorySalesProductDto();
      if (!item.isNonInventoryProductInvolved) {
        _a.id = item.id;
        _a.sequenceNumber = sequenceNumber++;
        _a.saleId = item.salesId;
        _a.salesNumber = item.salesNumber;
        _a.productId = item.productId;
        _a.code = item.productCode;
        _a.name = item.productName;
        _a.warehouseId = item.warehouseId;
        _a.warehouseCode = item.warehouseCode;
        _a.warehouseName = item.warehouseName;
        _a.sellingPrice = item.sellingPrice;
        _a.price = item.soldPrice;
        _a.discountRate = item.discountRate;
        _a.discountAmount = item.discountAmount;
        _a.quantity = item.quantity;
        _a.lineTotal = item.lineTotal;
        _a.remarks = item.remarks;
        _a.isActive = true;
        _header.inventorySalesProducts.push(_a);
      }

      let _b = new CreateNonInventorySalesProductDto();
      if (item.isNonInventoryProductInvolved) {
        console.log(item);
        _b.id = item.id;
        _b.sequenceNumber = sequenceNumber++;
        _b.saleId = item.tempSaleId;
        _b.salesNumber = item.salesNumber;
        _b.productId = item.productId;
        _b.productCode = item.productCode;
        _b.productName = item.productName;
        _b.warehouseId = item.warehouseId;
        _b.warehouseCode = item.warehouseCode;
        _b.warehouseName = item.warehouseName;
        _b.quantity = item.quantity;
        _b.quantityUnitOfMeasureUnit = item.quantityUnitOfMeasureUnit;
        _b.discountRate = item.discountRate;
        _b.discountAmount = item.discountAmount;
        _b.lineTotal = item.lineTotal;
        // _b.costPrice = item.nonInventorySalesProduct.costPrice;
        _b.sellingPrice = item.sellingPrice;
        // _b.maximumRetailPrice = item.nonInventorySalesProduct.maximumRetailPrice;
        _b.price = item.soldPrice;
        _header.nonInventorySalesProducts.push(_b);
      }

    });

    this._salesService.createOrUpdateSales(_header).subscribe((i) => {
      this.notify.info(this.l("SavedSuccessfully"));
      this.router.navigate(["/app/payment-component", i.id]);
    });
  }

  save() { }

  isNullOrEmptyString(stringVal: String): boolean {
    if (stringVal != undefined && stringVal != null && stringVal.length > 0) {
      return false;
    }
    return true;
  }

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
