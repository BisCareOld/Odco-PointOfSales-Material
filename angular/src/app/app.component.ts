import { Component, Injector, OnInit, Renderer2 } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { SignalRAspNetCoreHelper } from "@shared/helpers/SignalRAspNetCoreHelper";
import { LayoutStoreService } from "@shared/layout/layout-store.service";

import { BreakpointObserver, Breakpoints } from "@angular/cdk/layout";
import { Observable } from "rxjs";
import { map, shareReplay } from "rxjs/operators";

@Component({
  templateUrl: "./app.component.html",
})
export class AppComponent extends AppComponentBase implements OnInit {
  isDarkTheme: boolean = true;

  sidebarExpanded: boolean;

  isHandset$: Observable<boolean> = this.breakpointObserver
    .observe(Breakpoints.Handset)
    .pipe(
      map((result) => result.matches),
      shareReplay()
    );

  constructor(
    injector: Injector,
    private renderer: Renderer2,
    private _layoutStore: LayoutStoreService,
    private breakpointObserver: BreakpointObserver
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.isDarkTheme = localStorage.getItem("theme") === "Dark" ? true : false;

    this.renderer.addClass(document.body, "sidebar-mini");

    SignalRAspNetCoreHelper.initSignalR();

    abp.event.on("abp.notifications.received", (userNotification) => {
      abp.notifications.showUiNotifyForUserNotification(userNotification);

      // Desktop notification
      Push.create("AbpZeroTemplate", {
        body: userNotification.notification.data.message,
        icon: abp.appPath + "assets/app-logo-small.png",
        timeout: 6000,
        onClick: function () {
          window.focus();
          this.close();
        },
      });
    });

    this._layoutStore.sidebarExpanded.subscribe((value) => {
      this.sidebarExpanded = value;
    });
  }

  toggleSidebar(): void {
    this._layoutStore.setSidebarExpanded(!this.sidebarExpanded);
  }

  storeThemeSelection() {
    localStorage.setItem("theme", this.isDarkTheme ? "Dark" : "Light");
  }
}
