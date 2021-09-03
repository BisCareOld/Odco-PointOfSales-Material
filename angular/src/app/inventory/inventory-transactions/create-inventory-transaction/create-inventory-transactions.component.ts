import { Component, OnInit } from "@angular/core";
import { appModuleAnimation } from "@shared/animations/routerTransition";
@Component({
  selector: "app-create-inventory-transactions",
  templateUrl: "./create-inventory-transactions.component.html",
  styleUrls: ["./create-inventory-transactions.component.scss"],
  animations: [appModuleAnimation()],
})
export class CreateInventoryTransactionsComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {}

  save() {}
}
