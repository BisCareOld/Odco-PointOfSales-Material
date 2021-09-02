import { Component, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "shared/paged-listing-component-base";
import { MatDialog } from "@angular/material/dialog";
import {
  PurchasingServiceProxy,
  SupplierDto,
  SupplierDtoPagedResultDto,
} from "@shared/service-proxies/service-proxies";
import { CreateSupplierDialogComponent } from "./create-supplier/create-supplier-dialog.component";
import { EditSupplierDialogComponent } from "./edit-supplier/edit-supplier-dialog.component";

class PagedSupplierRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

@Component({
  selector: "app-suppliers",
  templateUrl: "./suppliers.component.html",
  styleUrls: ["./suppliers.component.scss"],
  animations: [appModuleAnimation()],
})
export class SuppliersComponent extends PagedListingComponentBase<SupplierDto> {
  suppliers: SupplierDto[] = [];
  keyword = "";
  isActive: boolean | null;
  advancedFiltersVisible = false;

  displayedColumns: string[] = ["name", "is-active", "actions"];
  dataSource;

  constructor(
    injector: Injector,
    private _purchasingService: PurchasingServiceProxy,
    private _matDialogService: MatDialog
  ) {
    super(injector);
  }

  protected list(
    request: PagedSupplierRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._purchasingService
      .getAllSuppliers(
        request.keyword,
        request.isActive,
        request.skipCount,
        request.maxResultCount
      )
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: SupplierDtoPagedResultDto) => {
        this.suppliers = result.items;
        this.dataSource = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  delete(supplier: SupplierDto): void {
    abp.message.confirm(
      this.l("RoleDeleteWarningMessage", supplier.firstName),
      undefined,
      (result: boolean) => {
        if (result) {
          this._purchasingService
            .deleteSupplier(supplier.id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.l("SuccessfullyDeleted"));
                this.refresh();
              })
            )
            .subscribe(() => {});
        }
      }
    );
  }

  createSupplier(): void {
    this.showCreateOrEditSupplierDialog();
  }

  editSupplier(category: SupplierDto): void {
    this.showCreateOrEditSupplierDialog(category.id);
  }

  showCreateOrEditSupplierDialog(id?: string): void {
    let materialDialog;
    if (!id) {
      materialDialog = this._matDialogService.open(
        CreateSupplierDialogComponent,
        {
          width: "50%",
        }
      );
    } else {
      materialDialog = this._matDialogService.open(
        EditSupplierDialogComponent,
        {
          data: { id: id },
          width: "50%",
        }
      );
    }
    materialDialog.afterClosed().subscribe(() => {
      this.refresh();
    });
  }
}
