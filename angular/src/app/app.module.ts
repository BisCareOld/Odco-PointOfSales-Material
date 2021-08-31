import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientJsonpModule } from "@angular/common/http";
import { HttpClientModule } from "@angular/common/http";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { ServiceProxyModule } from "@shared/service-proxies/service-proxy.module";
import { SharedModule } from "@shared/shared.module";
import { HomeComponent } from "@app/home/home.component";
import { AboutComponent } from "@app/about/about.component";

import { MaterialModule } from "../shared/material/material.module";

// tenants
import { TenantModule } from "./tenants/tenant.module";
// roles
import { RoleModule } from "./roles/role.module";
// users
import { UserModule } from "./users/user.module";
// product
import { ProductModule } from "./productions/products/product.module";
// category
import { CategoryModule } from "./productions/categories/category.module";
// layout
import { HeaderComponent } from "./layout/header.component";
import { HeaderLeftNavbarComponent } from "./layout/header-left-navbar.component";
import { HeaderLanguageMenuComponent } from "./layout/header-language-menu.component";
import { HeaderUserMenuComponent } from "./layout/header-user-menu.component";
import { FooterComponent } from "./layout/footer.component";
import { SidebarComponent } from "./layout/sidebar.component";
import { SidebarLogoComponent } from "./layout/sidebar-logo.component";
import { SidebarUserPanelComponent } from "./layout/sidebar-user-panel.component";
import { SidebarMenuComponent } from "./layout/sidebar-menu.component";
import { NavComponent } from "./layout/nav.component";
import { LayoutModule } from "@angular/cdk/layout";

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AboutComponent,
    // layout
    HeaderComponent,
    HeaderLeftNavbarComponent,
    HeaderLanguageMenuComponent,
    HeaderUserMenuComponent,
    FooterComponent,
    SidebarComponent,
    SidebarLogoComponent,
    SidebarUserPanelComponent,
    SidebarMenuComponent,
    NavComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    HttpClientJsonpModule,
    AppRoutingModule,
    ServiceProxyModule,
    MaterialModule,
    SharedModule,
    LayoutModule,
    UserModule,
    RoleModule,
    TenantModule,
    ProductModule,
    CategoryModule,
  ],
  providers: [],
  entryComponents: [],
})
export class AppModule {}
