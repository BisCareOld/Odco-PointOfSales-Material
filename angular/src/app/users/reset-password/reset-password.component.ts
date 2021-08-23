import { Component, OnInit, Injector, Inject } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { finalize } from "rxjs/operators";
import {
  UserServiceProxy,
  ResetPasswordDto,
} from "@shared/service-proxies/service-proxies";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-reset-password",
  templateUrl: "./reset-password.component.html",
  styleUrls: ["./reset-password.component.scss"],
})
export class ResetPasswordDialogComponent
  extends AppComponentBase
  implements OnInit
{
  public isLoading = false;
  public resetPasswordDto: ResetPasswordDto;

  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    public matDialogRef: MatDialogRef<ResetPasswordDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
  }

  ngOnInit() {
    this.isLoading = true;
    this.resetPasswordDto = new ResetPasswordDto();
    this.resetPasswordDto.userId = this.data.id;
    this.resetPasswordDto.newPassword = Math.random()
      .toString(36)
      .substr(2, 10);
    this.isLoading = false;
  }

  public resetPassword(): void {
    this.isLoading = true;
    this._userService
      .resetPassword(this.resetPasswordDto)
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe(() => {
        this.notify.info("Password Reset");
        this.matDialogRef.close();
      });
  }
}
