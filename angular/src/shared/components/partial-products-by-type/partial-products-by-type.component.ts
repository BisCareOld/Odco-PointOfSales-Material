import { Component, OnInit, Output, EventEmitter, Input } from "@angular/core";
import {
  ProductSearchResultDto,
  ProductionServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { Observable } from "rxjs";

@Component({
  selector: "partial-products-by-type",
  templateUrl: "./partial-products-by-type.component.html",
  styleUrls: ["./partial-products-by-type.component.scss"],
})
export class PartialProductsByTypeComponent implements OnInit {
  searchKeyword: string;
  filteredOptions: Observable<ProductSearchResultDto[]>;
  disable: boolean = false;

  @Input() type: number; // Enum ProductSearchType
  @Output() selectedEvent = new EventEmitter<ProductSearchResultDto>();

  constructor(private _productionService: ProductionServiceProxy) {}

  ngOnInit() {}

  changeKeyword() {
    this.filteredOptions = this._productionService.getPartialProductsByTypes(
      this.type,
      this.searchKeyword
    );
  }

  selectProduct(option: ProductSearchResultDto) {
    var vm = this;

    this.selectedEvent.emit(option);

    setTimeout(function () {
      vm.clearSelectedProduct();
    }, 1000);
  }

  clearSelectedProduct() {
    this.searchKeyword = null;
  }
}
