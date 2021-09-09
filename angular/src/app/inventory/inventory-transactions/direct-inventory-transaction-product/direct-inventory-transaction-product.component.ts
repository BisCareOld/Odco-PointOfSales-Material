import {
  Component,
  EventEmitter,
  Injector,
  Input,
  Output,
  OnInit,
} from "@angular/core";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { AppComponentBase } from "@shared/app-component-base";
import { finalize } from "rxjs/operators";
import {
  CommonKeyValuePairDto,
  CreateGoodsReceivedDto,
  CreateGoodsReceivedProductDto,
} from "@shared/service-proxies/service-proxies";

@Component({
  selector: "app-direct-inventory-transaction-product",
  templateUrl: "./direct-inventory-transaction-product.component.html",
  styleUrls: ["./direct-inventory-transaction-product.component.scss"],
})
export class DirectInventoryTransactionProductComponent implements OnInit {
  @Input() supplierId: string;
  @Output() grnProducts = new EventEmitter();
  @Output() isTableFormValid = new EventEmitter();

  createPurchaseOrder = new CreateGoodsReceivedDto();
  addresses: CommonKeyValuePairDto[] = [];
  supplier = new CommonKeyValuePairDto();
  products: CommonKeyValuePairDto[] = [];
  warehouses: CommonKeyValuePairDto[] = [];
  inventoryTransactionProducts: CreateGoodsReceivedProductDto[] = [];

  constructor() {}

  ngOnInit(): void {}
}
