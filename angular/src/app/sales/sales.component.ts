import { Component, OnInit } from "@angular/core";

@Component({
  selector: "app-sales",
  templateUrl: "./sales.component.html",
  styleUrls: ["./sales.component.scss"],
})
export class SalesComponent implements OnInit {
  selectedSearchProductType: number = 1;
  constructor() {}

  ngOnInit(): void {}
}
