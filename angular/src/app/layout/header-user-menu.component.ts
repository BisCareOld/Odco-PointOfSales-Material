import { Component, ChangeDetectionStrategy } from '@angular/core';
import { AppAuthService } from '@shared/auth/app-auth.service';

@Component({
  selector: 'header-user-menu',
  templateUrl: './header-user-menu.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderUserMenuComponent {
  constructor(private _authService: AppAuthService) { }

  //this.shownLoginName = this.appSession.getShownLoginName()     extends AppComponentBase

  logout(): void {
    this._authService.logout();
  }
}
