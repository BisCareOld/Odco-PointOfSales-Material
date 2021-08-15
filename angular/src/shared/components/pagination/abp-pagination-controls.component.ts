import { Component, Input, Output, EventEmitter } from "@angular/core";
import { PageEvent } from "@angular/material/paginator";

@Component({
  selector: "abp-pagination-controls",
  templateUrl: "./abp-pagination-controls.component.html",
})
export class AbpPaginationControlsComponent {
  @Input() id: string;
  @Input() maxSize = 7;
  @Input() previousLabel = "Previous";
  @Input() nextLabel = "Next";
  @Input() screenReaderPaginationLabel = "Pagination";
  @Input() screenReaderPageLabel = "page";
  @Input() screenReaderCurrentLabel = `You're on page`;
  @Output() pageChange: EventEmitter<number> = new EventEmitter<number>();

  private _directionLinks = true;
  private _autoHide = false;

  pageSizeOptions: number[] = [5, 10, 25, 100];

  @Input()
  get directionLinks(): boolean {
    return this._directionLinks;
  }
  set directionLinks(value: boolean) {
    this._directionLinks = !!value && <any>value !== "false";
  }
  @Input()
  get autoHide(): boolean {
    return this._autoHide;
  }
  set autoHide(value: boolean) {
    this._autoHide = !!value && <any>value !== "false";
  }

  // MatPaginator Output
  pageEvent: PageEvent;

  setPageSizeOptions(setPageSizeOptionsInput: string) {
    if (setPageSizeOptionsInput) {
      this.pageSizeOptions = setPageSizeOptionsInput
        .split(",")
        .map((str) => +str);
    }
  }
}
