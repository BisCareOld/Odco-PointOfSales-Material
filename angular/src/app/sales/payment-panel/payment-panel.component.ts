import { Component, Injector, OnInit, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import {
  SalesServiceProxy,
  CommonServiceProxy,
  SaleDto,
  CommonKeyValuePairDto,
} from "@shared/service-proxies/service-proxies";
import { ChequeDialogComponent } from "../payment-methods/cheque/cheque-dialog.component";
import { MatAccordion } from '@angular/material/expansion';
import { AppComponentBase } from "@shared/app-component-base";

@Component({
  selector: "app-payment-panel",
  templateUrl: "./payment-panel.component.html",
  styleUrls: ["./payment-panel.component.scss"],
})
export class PaymentPanelComponent extends AppComponentBase implements OnInit {
  @ViewChild(MatAccordion) accordion: MatAccordion;
  saleDto: SaleDto;
  paymentMethod: number = 0;
  formPayment;
  banks: CommonKeyValuePairDto[] = []
  errors: string[] = [];

  constructor(
    injector: Injector,
    private fb: FormBuilder,
    private _matDialogService: MatDialog,
    private _salesService: SalesServiceProxy,
    private _commonService: CommonServiceProxy,
    private router: Router,
    private route: ActivatedRoute
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._commonService.getAllBanks().subscribe((response) => {
      this.banks = response;
    });

    this.route.paramMap.subscribe((params) => {
      let tempSaleHeaderId = params.get("tempSalesId");
      let x = this._salesService
        .getSales(tempSaleHeaderId)
        .subscribe((response) => {
          this.saleDto = response;
          console.log(this.saleDto);
          this.InitateForm();
        });
    });
  }

  getNonInventoryProductBySaleId(saleId: string) {
    this._salesService.getNonInventoryProductBySaleId(saleId).subscribe((result) => {

    });
  }

  showChequeDialog(salesId: number, netAmount: number) {
    let materialDialog = this._matDialogService.open(ChequeDialogComponent, {
      data: { tempSalesId: salesId, netAmount: netAmount },
      width: "70%",
    });

    materialDialog.afterClosed().subscribe((result) => {
      // "SelectedProduct" came from Dialog
      if (result && result.event == "SelectedProduct") {
        //this.addProductToTable(netAmount, result.data);
      }
    });
  }

  InitateForm() {
    console.log(this.saleDto);
    this.formPayment = this.fb.group({
      id: [this.saleDto.id],
      salesNumber: [this.saleDto.salesNumber],
      referenceNumber: [this.saleDto.referenceNumber],
      customerId: [this.saleDto.customerId],
      customerCode: [this.saleDto.customerCode],
      customerName: [this.saleDto.customerName],
      discountRate: [this.saleDto.discountRate,
      Validators.compose([
        Validators.required,
        Validators.min(0),
        Validators.max(100),
      ]),
      ],
      discountAmount: [this.saleDto.discountAmount, Validators.required],
      taxRate: [this.saleDto.taxRate,
      Validators.compose([
        Validators.required,
        Validators.min(0),
        Validators.max(100),
      ]),
      ],
      taxAmount: [this.saleDto.taxAmount, Validators.required],
      grossAmount: [this.saleDto.grossAmount, Validators.required],
      netAmount: [this.saleDto.netAmount, Validators.required],
      receivedAmount: [0],
      balanceAmount: [0],
      remarks: [this.saleDto.remarks],
      isActive: [this.saleDto.isActive],
      inventorySalesProducts: [this.saleDto.inventorySalesProducts],
      nonInventorySalesProducts: [this.saleDto.nonInventorySalesProducts],
      cashes: this.fb.array([]),
      cheques: this.fb.array([]),
      outstandings: this.fb.array([]),
      debitCards: this.fb.array([]),
      giftCards: this.fb.array([]),
    });
  }

  navigateBack() {
    this.router.navigate(
      ["/app/sales"],
      {
        queryParams: { salesHeaderId: this.saleDto.id }
      });
  }

  paymentTypeSelection(paymentType: number) {
    this.EmptyArraysInForm();
    this.paymentMethod = paymentType;
    if (paymentType == 1) this.InitateCash();
    if (paymentType == 3) this.InitateCheque();
  }

  totalPaidAmout(): number {
    let totalAmount = 0;
    this.cashes.value.forEach(element => {
      totalAmount += element.cashAmount;
    });
    this.cheques.value.forEach(element => {
      totalAmount += element.chequeAmount;
    });
    return totalAmount;
  }

  calculateBalanceAmount(): number {
    let retunAmount = this.totalPaidAmout() - this.saleDto.netAmount;
    return +retunAmount.toFixed(2);
  }

  paymentValidation() {
    this.errors = [];
    let totalSum = 0;

    this.cashes.value.forEach(element => {
      totalSum += element.cashAmount;
    });
    this.cheques.value.forEach(element => {
      totalSum += element.chequeAmount;
    });
    if (this.cashes.length <= 0 && this.cheques.length <= 0) {
      this.errors.push("Select a Payment Method.");
    }
    if (this.cashes.length > 0 && totalSum <= 0) {
      this.errors.push("Invalid cash amount.");
    }
    if (this.cheques.length > 0 && totalSum <= 0) {
      this.errors.push("Invalid cheque amount.");
    }
    if (totalSum < this.netAmount.value) {
      this.errors.push("There is a shortage of Rs." + (this.netAmount.value - totalSum).toFixed(2));
    }

  }

  save() {
    this.errors = [];
    this.outstandings.clear();  // Only one is required, So clear the existing one & generate a new one

    let _totPaidAmount = this.totalPaidAmout();
    let _totBalanceAmount = this.calculateBalanceAmount();

    this.receivedAmount.setValue(_totPaidAmount);
    this.balanceAmount.setValue(_totBalanceAmount < 0 ? 0 : _totBalanceAmount);

    if (_totPaidAmount < this.netAmount.value && this.customerId.value) {

      let _outstandingAmount = Math.abs(_totBalanceAmount); // Make a negative number to positive

      abp.message.confirm(
        "An amount of Rs. " + _outstandingAmount.toFixed(2) + " for " + this.customerName.value,
        "Move to Customer Outstanding?",
        (result: boolean) => {
          if (result) {
            console.log(1);
            this.InitiateOutstanding(_outstandingAmount);
            this.submitPayment();

          } else {
            console.log(2)
            this.paymentValidation();
            this.submitPayment();
          }
        }
      );
    } else {
      console.log(3)
      this.paymentValidation();
      this.submitPayment();
    }
  }

  submitPayment() {
    if (this.errors.length <= 0) {
      console.log("Final", this.formPayment.value);
      this._salesService.createOrUpdateSales(this.formPayment.value).subscribe((i) => {
        this.notify.success(this.l("PaymentSuccessfullyCompleted"));
        //this.router.navigate(["/app/payment-component", i.id]);
      });
    }
  }

  multiSelectPayment(pType) {
    if (pType == 1) this.InitateCash();
    if (pType == 3) this.InitateCheque();
  }

  calculateLineLevelTotal() {
    let total = 0;
    console.log(this.inventorySalesProducts);
    this.inventorySalesProducts.value.forEach(function (item) {
      total += item.lineTotal;
    });
    this.nonInventorySalesProducts.value.forEach(function (item) {
      total += item.lineTotal;
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

  //#region Initiate array of Payment methods into main form
  InitateCash() {
    let cash = this.fb.group({
      cashAmount: [0,
        Validators.compose([
          Validators.required,
          Validators.min(1)
        ])],
    });
    this.cashes.push(cash);
  }

  InitateCheque() {
    let cheque = this.fb.group({
      chequeNumber: [null,
        Validators.compose([
          Validators.required,
          Validators.minLength(5),
          Validators.maxLength(25),
        ])],
      bankId: [null, Validators.required],
      bank: [null],
      branchId: [null], //Validators.required
      branch: [null],
      chequeReturnDate: [null, Validators.required],
      chequeAmount: [,
        Validators.compose([
          Validators.required,
          Validators.min(1)
        ])],

    });
    this.cheques.push(cheque);
  }

  InitiateOutstanding(outstandingAmount: number) {
    let o = this.fb.group({
      outstandingAmount: [outstandingAmount,
        Validators.compose([
          Validators.required,
          Validators.min(1)
        ])]
    });
    this.outstandings.push(o);
  }

  EmptyArraysInForm() {
    this.cashes.clear();
    this.cheques.clear();
    this.outstandings.clear();
    this.debitCards.clear();
    this.giftCards.clear();
  }
  //#endregion

  //#region Remove array of Payment methods in main form
  removeArrayInFormByIndex(paymentType: number, index: number) {
    if (this.paymentMethod != 6) {
      if (paymentType == 1) this.cashes.removeAt(index);
      if (paymentType == 3) this.cheques.removeAt(index);
      if (paymentType != 6) this.paymentMethod = 0;
    } else {
      if (paymentType == 1) this.cashes.removeAt(index);
      if (paymentType == 3) this.cheques.removeAt(index);
      if (this.cashes.length == 0 && this.cheques.length == 0) this.paymentMethod = 0;
    }
  }
  //#endregion

  //#region Propertises
  get id() {
    return this.formPayment.get("id") as FormControl;           // SaleId
  }

  get salesNumber() {
    return this.formPayment.get("salesNumber") as FormControl;
  }

  get referenceNumber() {
    return this.formPayment.get("referenceNumber") as FormControl;
  }

  get customerId() {
    return this.formPayment.get("customerId") as FormControl;
  }

  get customerCode() {
    return this.formPayment.get("customerCode") as FormControl;
  }

  get customerName() {
    return this.formPayment.get("customerName") as FormControl;
  }

  get discountRate() {
    return this.formPayment.get("discountRate") as FormControl;
  }

  get discountAmount() {
    return this.formPayment.get("discountAmount") as FormControl;
  }

  get taxRate() {
    return this.formPayment.get("taxRate") as FormControl;
  }

  get taxAmount() {
    return this.formPayment.get("taxAmount") as FormControl;
  }

  get grossAmount() {
    return this.formPayment.get("grossAmount") as FormControl;
  }

  get netAmount() {
    return this.formPayment.get("netAmount") as FormControl;
  }

  get receivedAmount() {
    return this.formPayment.get("receivedAmount") as FormControl;
  }

  get balanceAmount() {
    return this.formPayment.get("balanceAmount") as FormControl;
  }

  get remarks() {
    return this.formPayment.get("remarks") as FormControl;
  }

  get isActive() {
    return this.formPayment.get("IsActive") as FormControl;
  }

  get inventorySalesProducts(): FormArray {
    return this.formPayment.controls["inventorySalesProducts"] as FormArray;
  }

  get nonInventorySalesProducts(): FormArray {
    return this.formPayment.controls["nonInventorySalesProducts"] as FormArray;
  }

  //#region Payment
  get cashes(): FormArray {
    return this.formPayment.controls["cashes"] as FormArray;
  }

  get cheques(): FormArray {
    return this.formPayment.controls["cheques"] as FormArray;
  }

  get outstandings(): FormArray {
    return this.formPayment.controls["outstandings"] as FormArray;
  }

  get debitCards(): FormArray {
    return this.formPayment.controls["debitCards"] as FormArray;
  }

  get giftCards(): FormArray {
    return this.formPayment.controls["giftCards"] as FormArray;
  }
  //#endregion

  //#endregion

}
