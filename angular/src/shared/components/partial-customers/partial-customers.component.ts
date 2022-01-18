import { Component, OnInit, Output, EventEmitter, Input } from "@angular/core";
import {
  CustomerSearchResultDto,
  SalesServiceProxy,
} from "@shared/service-proxies/service-proxies";
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
  @Input('isDisabledWhenSelected') isDisabledWhenSelected: boolean = false;
  @Input('fieldRequired') fieldRequired: boolean = false;
  @Output() selectedEvent = new EventEmitter<CustomerSearchResultDto>();

  constructor(private _salesService: SalesServiceProxy) { }

  ngOnInit(): void { }

  changeKeyword() {
    this.filteredOptions = this._salesService.getPartialCustomers(
      this.searchKeyword
    );
  }

  selectCustomer(option: CustomerSearchResultDto) {
    var vm = this;

    this.selectedEvent.emit(option);
    if (this.isDisabledWhenSelected) {
      this.disableInput(true);
    }

    if (this.isClearWhenSelected) {
      setTimeout(function () {
        vm.clearSelectedCustomer();
      }, 1000);
    }
  }

  private clearSelectedCustomer() {
    this.searchKeyword = null;
    this.selectedEvent.emit(null);
    this.disableInput(false);
  }

  private disableInput(input: boolean) {
    this.disable = input;
  }

}
