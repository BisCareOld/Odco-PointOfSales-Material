import { Component, Input, Output, EventEmitter } from "@angular/core";
import { PageEvent } from "@angular/material/paginator";

@Component({
  selector: "abp-pagination-controls",
  templateUrl: "./abp-pagination-controls.component.html",
})
export class AbpPaginationControlsComponent {
  @Input() totalItems: number;
  @Output() pageChange: EventEmitter<any> = new EventEmitter();

  pageSizeOptions: number[] = [5, 10, 25, 100];

  onChangePage(event: PageEvent) {
    this.pageChange.emit({
      pageIndex: event.pageIndex + 1,
      pageSize: event.pageSize,
    });
  }
}
