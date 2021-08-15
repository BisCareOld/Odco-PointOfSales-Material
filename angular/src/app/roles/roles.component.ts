import { Component, Injector } from "@angular/core";
import { finalize } from "rxjs/operators";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import {
  RoleServiceProxy,
  RoleDto,
  RoleDtoPagedResultDto,
} from "@shared/service-proxies/service-proxies";
// import { CreateRoleDialogComponent } from "./create-role/create-role-dialog.component";
// import { EditRoleDialogComponent } from "./edit-role/edit-role-dialog.component";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
//import { MatEditRoleComponent } from "./mat-edit-role/mat-edit-role.component";

class PagedRolesRequestDto extends PagedRequestDto {
  keyword: string;
}

@Component({
  templateUrl: "./roles.component.html",
  styleUrls: ["./roles.component.css"],
  animations: [appModuleAnimation()],
})
export class RolesComponent extends PagedListingComponentBase<RoleDto> {
  roles: RoleDto[] = [];
  keyword = "";

  displayedColumns: string[] = ["name", "display-name", "actions"];
  dataSource;

  constructor(
    injector: Injector,
    private _rolesService: RoleServiceProxy,
    private _modalService: BsModalService,
    private _matDialogService: MatDialog
  ) {
    super(injector);
  }

  list(
    request: PagedRolesRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;

    this._rolesService
      .getAll(request.keyword, request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: RoleDtoPagedResultDto) => {
        this.roles = result.items;

        this.dataSource = result.items;

        this.showPaging(result, pageNumber);
      });
  }

  delete(role: RoleDto): void {
    abp.message.confirm(
      this.l("RoleDeleteWarningMessage", role.displayName),
      undefined,
      (result: boolean) => {
        if (result) {
          this._rolesService
            .delete(role.id)
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

  createRole(): void {
    this.showCreateOrEditRoleDialog();
  }

  editRole(role: RoleDto): void {
    this.showCreateOrEditRoleDialog(role.id);
  }

  showCreateOrEditRoleDialog(id?: number): void {
    //let createOrEditRoleDialog: BsModalRef;
    //let createOrEditRoleDialog: MatDialogRef<>;

    if (!id) {
      // let createOrEditRoleDialog = this._modalService.show(
      //   CreateRoleDialogComponent,
      //   {
      //     class: "modal-lg",
      //   }
      // );
    } else {
      // let createOrEditRoleDialog = this._matDialogService.open(
      //   EditRoleDialogComponent,
      //   {
      //     class: "modal-lg",
      //     initialState: {
      //       id: id,
      //     },
      //   }
      // );
      // let createOrEditRoleDialog = this._matDialogService.open(
      //   EditRoleDialogComponent,
      //   { data: { id: id } }
      // );
      // let createOrEditRoleDialog =
      //   this._matDialogService.open(MatEditRoleComponent);
    }

    // let createOrEditRoleDialog.content.onSave.subscribe(() => {
    //   this.refresh();
    // });
  }
}
