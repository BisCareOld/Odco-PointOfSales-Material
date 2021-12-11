import { ThrowStmt } from "@angular/compiler";
import { Component, OnChanges, OnInit } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import {
  SalesServiceProxy,
  CommonServiceProxy,
  FinanceServiceProxy,
  TempSaleDto,
  CommonKeyValuePairDto,
} from "@shared/service-proxies/service-proxies";
import { result } from "lodash-es";
import { ChequeDialogComponent } from "../payment-methods/cheque/cheque-dialog.component";

@Component({
  selector: "app-payment-panel",
  templateUrl: "./payment-panel.component.html",
  styleUrls: ["./payment-panel.component.scss"],
})
export class PaymentPanelComponent implements OnInit {
  tempSalesHeader: TempSaleDto;
  paymentMethod: number = 0;
  formPayment;
  banks: CommonKeyValuePairDto[] = []
  errors: string[] = [];

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private _matDialogService: MatDialog,
    private _salesService: SalesServiceProxy,
    private _commonService: CommonServiceProxy,
    private _financeService: FinanceServiceProxy,
  ) { }

  ngOnInit(): void {
    this._commonService.getAllBanks().subscribe((response) => {
      this.banks = response;
    });

    this.route.paramMap.subscribe((params) => {
      let tempSaleHeaderId = +params.get("tempSalesId");
      let x = this._salesService
        .getTempSales(tempSaleHeaderId)
        .subscribe((response) => {
          this.tempSalesHeader = response;
          console.log(this.tempSalesHeader);
          this.InitateForm();
        });
    });
  }

  getNonInventoryProductByTempSaleId(tempSaleId: number) {
    this._salesService.getNonInventoryProductByTempSaleId(tempSaleId).subscribe((result) => {

    })
  }

  showChequeDialog(tempSalesId: number, netAmount: number) {
    let materialDialog = this._matDialogService.open(ChequeDialogComponent, {
      data: { tempSalesId: tempSalesId, netAmount: netAmount },
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

    this.formPayment = this.fb.group({
      tempSalesHeaderId: [this.tempSalesHeader.id],
      customerId: [this.tempSalesHeader.customerId],
      customerCode: [this.tempSalesHeader.customerCode],
      customerName: [this.tempSalesHeader.customerName],
      discountRate: [this.tempSalesHeader.discountRate,
      Validators.compose([
        Validators.required,
        Validators.min(0),
        Validators.max(100),
      ]),
      ],
      discountAmount: [this.tempSalesHeader.discountAmount, Validators.required],
      taxRate: [this.tempSalesHeader.taxRate,
      Validators.compose([
        Validators.required,
        Validators.min(0),
        Validators.max(100),
      ]),
      ],
      taxAmount: [this.tempSalesHeader.taxAmount, Validators.required],
      grossAmount: [this.tempSalesHeader.grossAmount, Validators.required],
      netAmount: [this.tempSalesHeader.netAmount, Validators.required],
      cashes: this.fb.array([]),
      cheques: this.fb.array([]),
      outstandings: this.fb.array([]),
      debitCards: this.fb.array([]),
      giftCards: this.fb.array([]),
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

  balanceAmount() {
    let retunAmount = this.totalPaidAmout() - this.tempSalesHeader.netAmount;
    return retunAmount.toFixed(2);
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
  }

  save() {
    this.paymentValidation();
    this.totalPaidAmout();

    if (this.errors.length <= 0) {
      console.log(this.formPayment);
      // this._financeService.createInvoice(this.formPayment.value).subscribe((response) => {
      //   if (response.id) {
      //     alert("Success");
      //   }
      // });
    }
  }

  multiSelectPayment(pType) {
    if (pType == 1) this.InitateCash();
    if (pType == 3) this.InitateCheque();
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
      branchId: [null, Validators.required],
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
  get tempSalesHeaderId() {
    return this.formPayment.get("tempSalesHeaderId") as FormControl;
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

}
