import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import {
  SalesServiceProxy,
  TempSalesHeaderDto,
} from "@shared/service-proxies/service-proxies";

@Component({
  selector: "app-payment-panel",
  templateUrl: "./payment-panel.component.html",
  styleUrls: ["./payment-panel.component.scss"],
})
export class PaymentPanelComponent implements OnInit {
  tempSalesHeader: TempSalesHeaderDto;

  constructor(
    private route: ActivatedRoute,
    private _salesService: SalesServiceProxy
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      let tempSaleHeaderId = +params.get("tempSalesId");
      let x = this._salesService
        .getTempSales(tempSaleHeaderId)
        .subscribe((response) => {
          this.tempSalesHeader = response;
          console.log(this.tempSalesHeader);
        });
    });
  }
}
