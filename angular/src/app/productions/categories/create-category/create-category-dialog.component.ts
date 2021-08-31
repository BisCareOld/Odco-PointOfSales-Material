import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
} from "@angular/core";
import { finalize } from "rxjs/operators";
import { AppComponentBase } from "@shared/app-component-base";
import {
  ProductionServiceProxy,
  CategoryDto,
  CreateCategoryDto,
} from "@shared/service-proxies/service-proxies";
import { forEach as _forEach, map as _map } from "lodash-es";
import { MatDialogRef } from "@angular/material/dialog";

@Component({
  selector: "app-create-category-dialog",
  templateUrl: "./create-category-dialog.component.html",
  styleUrls: ["./create-category-dialog.component.scss"],
})
export class CreateCategoryDialogComponent extends AppComponentBase {
  saving = false;
  category = new CategoryDto();

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    public matDialogRef: MatDialogRef<CreateCategoryDialogComponent>
  ) {
    super(injector);
  }

  save(): void {
    this.saving = true;

    const _category = new CreateCategoryDto();
    _category.init(this.category);

    this._productionService
      .createCategory(_category)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l("SavedSuccessfully"));
        this.matDialogRef.close();
        this.onSave.emit();
      });
  }
}
