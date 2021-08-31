import { Component, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "shared/paged-listing-component-base";
import { MatDialog } from "@angular/material/dialog";
import {
  ProductionServiceProxy,
  CategoryDto,
  CategoryDtoPagedResultDto,
} from "@shared/service-proxies/service-proxies";
import { CreateCategoryDialogComponent } from "./create-category/create-category-dialog.component";
import { EditCategoryDialogComponent } from "./edit-category/edit-category-dialog.component";

class PagedCategoryRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

@Component({
  selector: "app-categories",
  templateUrl: "./categories.component.html",
  styleUrls: ["./categories.component.scss"],
  animations: [appModuleAnimation()],
})
export class CategoriesComponent extends PagedListingComponentBase<CategoryDto> {
  categories: CategoryDto[] = [];
  keyword = "";
  isActive: boolean | null;
  advancedFiltersVisible = false;

  displayedColumns: string[] = ["name", "is-active", "actions"];
  dataSource;

  constructor(
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    private _matDialogService: MatDialog
  ) {
    super(injector);
  }

  protected list(
    request: PagedCategoryRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._productionService
      .getAllCategories(
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
      .subscribe((result: CategoryDtoPagedResultDto) => {
        this.categories = result.items;
        this.dataSource = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  delete(product: CategoryDto): void {
    abp.message.confirm(
      this.l("RoleDeleteWarningMessage", product.name),
      undefined,
      (result: boolean) => {
        if (result) {
          this._productionService
            .deleteCategory(product.id)
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

  createCategory(): void {
    this.showCreateOrEditCategoryDialog();
  }

  editCategory(category: CategoryDto): void {
    this.showCreateOrEditCategoryDialog(category.id);
  }

  showCreateOrEditCategoryDialog(id?: string): void {
    let materialDialog;
    if (!id) {
      materialDialog = this._matDialogService.open(
        CreateCategoryDialogComponent,
        {
          width: "50%",
        }
      );
    } else {
      materialDialog = this._matDialogService.open(
        EditCategoryDialogComponent,
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
