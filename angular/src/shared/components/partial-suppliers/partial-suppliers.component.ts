import { Component, OnInit, Output, EventEmitter } from "@angular/core";
import {
  CommonKeyValuePairDto,
  PurchasingServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { Observable } from "rxjs";

@Component({
  selector: "partial-suppliers",
  templateUrl: "./partial-suppliers.component.html",
  styleUrls: ["./partial-suppliers.component.scss"],
})
export class PartialSuppliersComponent implements OnInit {
  constructor(private _purchasingService: PurchasingServiceProxy) {}

  searchKeyword: string;
  filteredOptions: Observable<CommonKeyValuePairDto[]>;
  disable: boolean = false;

  @Output() selectedEvent = new EventEmitter<CommonKeyValuePairDto>();

  ngOnInit() {}

  changeKeyword() {
    this.filteredOptions = this._purchasingService.getPartialSuppliers(
      this.searchKeyword
    );
  }

  selectSupplier(option: CommonKeyValuePairDto) {
    this.disable = true;
    this.selectedEvent.emit(option);
  }

  clearSelectedSupplier() {
    this.searchKeyword = null;
    this.disable = false;
    this.selectedEvent.emit(new CommonKeyValuePairDto());
  }
}
