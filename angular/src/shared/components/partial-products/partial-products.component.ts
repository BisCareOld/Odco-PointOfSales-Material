import { Component, OnInit, Output, EventEmitter } from "@angular/core";
import {
  CommonKeyValuePairDto,
  ProductionServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { Observable } from "rxjs";

@Component({
  selector: "partial-products",
  templateUrl: "./partial-products.component.html",
  styleUrls: ["./partial-products.component.scss"],
})
export class PartialProductsComponent implements OnInit {
  constructor(private _productionService: ProductionServiceProxy) {}

  searchKeyword: string;
  filteredOptions: Observable<CommonKeyValuePairDto[]>;
  disable: boolean = false;

  @Output() selectedEvent = new EventEmitter<CommonKeyValuePairDto>();

  ngOnInit() {}

  changeKeyword() {
    this.filteredOptions = this._productionService.getPartialProducts(
      this.searchKeyword
    );
  }

  selectProduct(option: CommonKeyValuePairDto) {
    this.selectedEvent.emit(option);
  }

  clearSelectedProduct() {
    this.searchKeyword = null;
  }
}
