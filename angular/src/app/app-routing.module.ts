import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { AppComponent } from "./app.component";
import { AppRouteGuard } from "@shared/auth/auth-route-guard";
import { HomeComponent } from "./home/home.component";
import { AboutComponent } from "./about/about.component";
import { UsersComponent } from "./users/users.component";
import { TenantsComponent } from "./tenants/tenants.component";
import { RolesComponent } from "app/roles/roles.component";
import { ProductsComponent } from "./productions/products/products.component";
import { ChangePasswordComponent } from "./users/change-password/change-password.component";
import { CategoriesComponent } from "./productions/categories/categories.component";
import { BrandsComponent } from "./productions/brands/brands.component";
import { SuppliersComponent } from "./purchasings/suppliers/suppliers.component";
import { InventoryTransactionsComponent } from "./inventory/inventory-transactions/inventory-transactions.component";
import { CreateInventoryTransactionsComponent } from "./inventory/inventory-transactions/create-inventory-transaction/create-inventory-transactions.component";
import { CreatePurchaseOrderComponent } from "./purchasings/purchase-orders/create-purchase-order/create-purchase-order.component";
import { SalesComponent } from "./sales/sales.component";
import { PaymentPanelComponent } from "./sales/payment-panel/payment-panel.component";
import { TempSalesListComponent } from "./sales/temp-sales-list/temp-sales-list.component";
import { CreateOutstandingPaymentComponent } from "./payments/create-payment-for-customer-outstanding/create-outstanding-payment.component";

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: "",
        component: AppComponent,
        children: [
          {
            path: "home",
            component: HomeComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "users",
            component: UsersComponent,
            data: { permission: "Pages.Users" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "roles",
            component: RolesComponent,
            data: { permission: "Pages.Roles" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "tenants",
            component: TenantsComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "about",
            component: AboutComponent,
            canActivate: [AppRouteGuard],
          },
          {
            path: "products",
            component: ProductsComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "categories",
            component: CategoriesComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "brands",
            component: BrandsComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "suppliers",
            component: SuppliersComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "create-purchase-orders",
            component: CreatePurchaseOrderComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "inventory-transactions",
            component: InventoryTransactionsComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "create-inventory-transactions",
            component: CreateInventoryTransactionsComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "sales",
            component: SalesComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "payment-component/:tempSalesId",
            component: PaymentPanelComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "sales-list",
            component: TempSalesListComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "create-customer-outstanding-payment",
            component: CreateOutstandingPaymentComponent,
            data: { permission: "Pages.Tenants" },
            canActivate: [AppRouteGuard],
          },
          {
            path: "update-password",
            component: ChangePasswordComponent,
            canActivate: [AppRouteGuard],
          },
        ],
      },
    ]),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule { }
