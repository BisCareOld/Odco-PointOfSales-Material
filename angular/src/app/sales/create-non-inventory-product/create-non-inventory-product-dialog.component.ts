import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { CommonKeyValuePairDto, ProductionServiceProxy, ProductStockBalanceDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-non-inventory-product-dialog',
  templateUrl: './create-non-inventory-product-dialog.component.html',
  styleUrls: ['./create-non-inventory-product-dialog.component.scss']
})
export class CreateNonInventoryProductDialogComponent implements OnInit {
  nipForm;
  warehouses: CommonKeyValuePairDto[] = [];
  recentlyCreatedGRN: ProductStockBalanceDto;
  constructor(private fb: FormBuilder,
    private _productionService: ProductionServiceProxy,
    public matDialogRef: MatDialogRef<CreateNonInventoryProductDialogComponent>) { }

  ngOnInit(): void {
    this.nipForm = this.fb.group({
      sequenceNumber: [1, Validators.required],
      tempSaleId: [null],
      productId: [null, Validators.required],
      productCode: [null, Validators.required],
      productName: [null, Validators.required],
      warehouseId: [null, Validators.required],
      warehouseCode: [null, Validators.required],
      warehouseName: [null, Validators.required],
      quantity: [0, Validators.compose([
        Validators.required,
        Validators.min(1),
      ])],
      quantityUnitOfMeasureUnit: [null],
      discountRate: [0],
      discountAmount: [0],
      lineTotal: [0],
      costPrice: [0, Validators.compose([
        Validators.required,
        Validators.min(1),
      ])],
      sellingPrice: [0, Validators.compose([
        Validators.required,
        Validators.min(1),
      ])],
      maximumRetailPrice: [0, Validators.compose([
        Validators.required,
        Validators.min(1),
      ])],
    });

    this._productionService.getAllKeyValuePairWarehouses().subscribe((result) => {
      this.warehouses = result;
    })
  }

  selectedProducts($event: CommonKeyValuePairDto) {
    this.productId.setValue($event.id);
    this.productCode.setValue($event.code);
    this.productName.setValue($event.name);
    console.log(this.nipForm)
    this.getRecentlyCreatedGRN($event.id);
  }

  getRecentlyCreatedGRN(productId) {
    this._productionService.getRecentlyCreatedGoodsReceivedNote(productId).subscribe((result) => {
      this.recentlyCreatedGRN = result;
      console.log(result)
      if (this.recentlyCreatedGRN != undefined && this.recentlyCreatedGRN != null) {
        this.costPrice.setValue(this.recentlyCreatedGRN.costPrice);
        this.sellingPrice.setValue(this.recentlyCreatedGRN.sellingPrice);
        this.maximumRetailPrice.setValue(this.recentlyCreatedGRN.maximumRetailPrice);
      }
    });
  }

  selectedWarehouse($event) {
    this.warehouseId.setValue($event.value.id);
    this.warehouseCode.setValue($event.value.code);
    this.warehouseName.setValue($event.value.name);
    console.log(this.nipForm)
  }

  submit() {
    this.lineTotal.setValue(this.quantity.value * this.sellingPrice.value);

    this.matDialogRef.close({
      data: this.nipForm.value,
      event: "NonInventoryProduct",
    });
  }

  //#region Propertises
  get sequenceNumber() {
    return this.nipForm.get("sequenceNumber") as FormControl;
  }
  get tempSaleId() {
    return this.nipForm.get("tempSaleId") as FormControl;
  }
  get productId() {
    return this.nipForm.get("productId") as FormControl;
  }
  get productName() {
    return this.nipForm.get("productName") as FormControl;
  }
  get productCode() {
    return this.nipForm.get("productCode") as FormControl;
  }
  get warehouseId() {
    return this.nipForm.get("warehouseId") as FormControl;
  }
  get warehouseCode() {
    return this.nipForm.get("warehouseCode") as FormControl;
  }
  get warehouseName() {
    return this.nipForm.get("warehouseName") as FormControl;
  }
  get quantity() {
    return this.nipForm.get("quantity") as FormControl;
  }
  get quantityUnitOfMeasureUnit() {
    return this.nipForm.get("quantityUnitOfMeasureUnit") as FormControl;
  }
  get discountRate() {
    return this.nipForm.get("discountRate") as FormControl;
  }
  get discountAmount() {
    return this.nipForm.get("discountAmount") as FormControl;
  }
  get lineTotal() {
    return this.nipForm.get("lineTotal") as FormControl;
  }
  get costPrice() {
    return this.nipForm.get("costPrice") as FormControl;
  }
  get sellingPrice() {
    return this.nipForm.get("sellingPrice") as FormControl;
  }
  get maximumRetailPrice() {
    return this.nipForm.get("maximumRetailPrice") as FormControl;
  }
  //#endregion
}
