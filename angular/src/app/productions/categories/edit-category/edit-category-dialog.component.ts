import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
  Inject,
} from "@angular/core";
import { finalize } from "rxjs/operators";
import {
  forEach as _forEach,
  includes as _includes,
  map as _map,
} from "lodash-es";
import { AppComponentBase } from "@shared/app-component-base";
import {
  ProductionServiceProxy,
  ProductDto,
  CategoryDto,
} from "@shared/service-proxies/service-proxies";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-edit-category-dialog",
  templateUrl: "./edit-category-dialog.component.html",
  styleUrls: ["./edit-category-dialog.component.scss"],
})
export class EditCategoryDialogComponent
  extends AppComponentBase
  implements OnInit
{
  saving = false;
  category = new CategoryDto();

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _productionService: ProductionServiceProxy,
    public matDialogRef: MatDialogRef<EditCategoryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._productionService
      .getCategory(this.data.id)
      .subscribe((result: ProductDto) => {
        this.category = result;
      });
  }

  save(): void {
    this.saving = true;

    const _category = new CategoryDto();
    _category.init(this.category);

    this._productionService
      .updateCategory(_category)
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
