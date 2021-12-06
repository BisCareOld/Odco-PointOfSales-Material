import { Component, OnInit, Output, EventEmitter, Input } from "@angular/core";
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
  searchKeyword: string;
  filteredOptions: Observable<CommonKeyValuePairDto[]>;
  disable: boolean = false;

  @Input('clearSelectedField') clearSelectedField: boolean = false; // Enum ProductSearchType
  @Output() selectedEvent = new EventEmitter<CommonKeyValuePairDto>();

  constructor(private _productionService: ProductionServiceProxy) { }

  ngOnInit() { }

  changeKeyword() {
    this.filteredOptions = this._productionService.getPartialProducts(
      this.searchKeyword
    );
  }

  selectProduct(option: CommonKeyValuePairDto) {
    var vm = this;

    this.selectedEvent.emit(option);
    console.log(this.clearSelectedField)
    if (!this.clearSelectedField) {
      setTimeout(function () {
        vm.clearSelectedProduct();
      }, 1000);
    }

  }

  clearSelectedProduct() {
    this.searchKeyword = null;
  }
}
