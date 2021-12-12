import { Component, OnInit, Output, EventEmitter, Input } from "@angular/core";
import {
  CustomerSearchResultDto,
  ProductionServiceProxy,
  SalesServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { result } from "lodash-es";
import { Observable } from "rxjs";

@Component({
  selector: 'app-partial-customers',
  templateUrl: 'partial-customers.component.html',
  styleUrls: ['./partial-customers.component.scss']
})
export class PartialCustomersComponent implements OnInit {
  searchKeyword: string;
  filteredOptions: Observable<CustomerSearchResultDto[]>;
  disable: boolean = false;

  @Input('isClearWhenSelected') isClearWhenSelected: boolean = false;
  @Input('fieldDisabled') fieldDisabled: boolean = false;
  @Input('fieldRequired') fieldRequired: boolean = false;
  @Output() selectedEvent = new EventEmitter<CustomerSearchResultDto>();

  constructor(private _salesService: SalesServiceProxy) { }

  ngOnInit(): void { }

  changeKeyword() {
    console.log(1)
    this.filteredOptions = this._salesService.getPartialCustomers(
      this.searchKeyword
    );
  }

  selectCustomer(option: CustomerSearchResultDto) {
    console.log(2)
    var vm = this;

    this.selectedEvent.emit(option);
    console.log(this.isClearWhenSelected)
    if (!this.isClearWhenSelected) {
      setTimeout(function () {
        vm.clearSelectedCustomer();
      }, 1000);
    }

  }

  clearSelectedCustomer() {
    this.searchKeyword = null;
  }

}
