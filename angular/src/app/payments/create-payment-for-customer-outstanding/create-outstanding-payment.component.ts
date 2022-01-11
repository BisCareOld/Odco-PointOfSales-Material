import { SelectionModel } from '@angular/cdk/collections';
import { Component, Injector, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { AppComponentBase } from '@shared/app-component-base';
import { SalesServiceProxy, CustomerSearchResultDto, OutstandingSaleDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-outstanding-payment',
  templateUrl: './create-outstanding-payment.component.html',
  styleUrls: ['./create-outstanding-payment.component.scss']
})
export class CreateOutstandingPaymentComponent extends AppComponentBase implements OnInit {
  paymentForm;
  displayedColumns: string[] = [
    "is-selected",
    "position",
    "sales-number",
    "net-amount",
    "paid-amount",
    "due-outstanding-amount",
    "entered-amount",
  ];
  dataSource = new MatTableDataSource<FormGroup>();

  constructor(
    injector: Injector,
    private fb: FormBuilder,
    private _salesService: SalesServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm() {
    this.paymentForm = this.fb.group({
      customerId: [null, Validators.required],
      customerCode: [null, Validators.required],
      customerName: [null, Validators.required],
      totalReceivedAmount: [0, Validators.required],
      totalBalanceAmount: [0, Validators.required],
      IsOutstandingPaymentInvolved: [true],
      remarks: [null, Validators.maxLength(100)],
      outstandingSales: this.fb.array([]),
      cashes: this.fb.array([]),
      cheques: this.fb.array([]),
      debitCards: this.fb.array([]),
      giftCards: this.fb.array([]),
    });
  }

  private initializeOutstandingSalesForm(
    os: OutstandingSaleDto,
  ) {
    let item = this.fb.group({
      isSelected: [os.isSelected],
      salesId: [os.saleId, Validators.required],
      salesNumber: [os.salesNumber, Validators.required],
      netAmount: [os.netAmount.toFixed(2)],
      paidAmount: [(os.netAmount - os.dueOutstandingAmount).toFixed(2)],    // only for showing in UI
      dueOutstandingAmount: [os.dueOutstandingAmount.toFixed(2)],
      enteredAmount: [os.enteredAmount.toFixed(2), Validators.max(os.netAmount - os.dueOutstandingAmount)],
    });
    this.outstandingSales.push(item);
    this.dataSource.data.push(item);
    return (this.dataSource.filter = "");
  }

  selectCustomer($event: CustomerSearchResultDto) {
    if ($event != undefined && $event != null) {
      this.setValuesForCustomer($event.id, $event.code, $event.name);
      this.getOutstandingSalesByCustomerId($event.id);
    } else {
      this.setValuesForCustomer(null, null, null);
      this.dataSource.data = [];
    }
  }

  private setValuesForCustomer(id, code, name) {
    this.customerId.setValue(id);
    this.customerCode.setValue(code);
    this.customerName.setValue(name);
  }

  private getOutstandingSalesByCustomerId(customerId) {
    this._salesService.getOutstandingSalesByCustomerId(customerId).subscribe((results) => {
      results.forEach((result: OutstandingSaleDto) => this.initializeOutstandingSalesForm(result));
    });
  }

  //#region outstandingSales => isSelected
  isSelectToggle(element: FormGroup) {
    let x = element.get("isSelected").value;
    element.get("isSelected").setValue(!x);
  }

  isSalesSelected(element: FormGroup): boolean {
    return element.get("isSelected").value;
  }

  isAnySalesSelected(): boolean {
    return this.dataSource.data.filter(x => x.get("isSelected").value == true).length > 0 ? true : false;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected(): boolean {
    const numSelected = this.dataSource.data.filter((x) => x.get("isSelected").value == true).length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.dataSource.data.forEach((x) => x.get("isSelected").setValue(false)) :
      this.dataSource.data.forEach((x) => x.get("isSelected").setValue(true));
  }
  //#endregion

  save() {

  }

  //#region Propertises
  get customerId() {
    return this.paymentForm.get("customerId") as FormControl;
  }

  get customerCode() {
    return this.paymentForm.get("customerCode") as FormControl;
  }

  get customerName() {
    return this.paymentForm.get("customerName") as FormControl;
  }

  get totalReceivedAmount() {
    return this.paymentForm.get("totalReceivedAmount") as FormControl;
  }

  get totalBalanceAmount() {
    return this.paymentForm.get("totalBalanceAmount") as FormControl;
  }

  get isOutstandingPaymentInvolved() {
    return this.paymentForm.get("isOutstandingPaymentInvolved") as FormControl;
  }

  get remarks() {
    return this.paymentForm.get("remarks") as FormControl;
  }

  get outstandingSales(): FormArray {
    return this.paymentForm.controls["outstandingSales"] as FormArray;
  }

  get cashes(): FormArray {
    return this.paymentForm.controls["cashes"] as FormArray;
  }

  get cheques(): FormArray {
    return this.paymentForm.controls["cheques"] as FormArray;
  }

  get debitCards(): FormArray {
    return this.paymentForm.controls["debitCards"] as FormArray;
  }

  get giftCards(): FormArray {
    return this.paymentForm.controls["giftCards"] as FormArray;
  }
  //#endregion
}
