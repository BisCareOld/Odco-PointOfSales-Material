import {
  Component,
  OnInit,
  SimpleChanges,
  OnChanges,
  Output,
  EventEmitter,
} from "@angular/core";
import { FormControl } from "@angular/forms";
import {
  CommonKeyValuePairDto,
  PurchasingServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { Observable } from "rxjs";
import { map, startWith } from "rxjs/operators";
import { finalize } from "rxjs/operators";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "shared/paged-listing-component-base";
import { MatDialog } from "@angular/material/dialog";
import { MatAutocompleteSelectedEvent } from "@angular/material/autocomplete";

@Component({
  selector: "app-partial-suppliers",
  templateUrl: "./partial-suppliers.component.html",
  styleUrls: ["./partial-suppliers.component.scss"],
})
export class PartialSuppliersComponent implements OnInit {
  constructor(private _purchasingService: PurchasingServiceProxy) {}
  searchKeyword: string;
  filteredOptions: Observable<CommonKeyValuePairDto[]>;
  disable: boolean = false;

  @Output() optionSelected: EventEmitter<MatAutocompleteSelectedEvent>;

  ngOnInit() {}

  changeKeyword() {
    console.log(this.searchKeyword);
    this.filteredOptions = this._purchasingService.getPartialSuppliers(
      this.searchKeyword
    );
  }

  selectedSupplier(option: CommonKeyValuePairDto) {
    console.log(option);
    this.disable = true;
  }

  clearSelectedSupplier() {
    this.searchKeyword = null;
    this.disable = false;
  }
}
