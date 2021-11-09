import { Component, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import {
  SalesServiceProxy,
  TempSalesHeaderDto,
} from "@shared/service-proxies/service-proxies";
import { ChequeDialogComponent } from "../payment-methods/cheque/cheque-dialog.component";

@Component({
  selector: "app-payment-panel",
  templateUrl: "./payment-panel.component.html",
  styleUrls: ["./payment-panel.component.scss"],
})
export class PaymentPanelComponent implements OnInit {
  tempSalesHeader: TempSalesHeaderDto;

  constructor(
    private route: ActivatedRoute,
    private _matDialogService: MatDialog,
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

  showChequeDialog(tempSalesId: number, netAmount: number) {
    let materialDialog = this._matDialogService.open(ChequeDialogComponent, {
      data: { tempSalesId: tempSalesId, netAmount: netAmount },
      width: "70%",
    });

    materialDialog.afterClosed().subscribe((result) => {
      // "SelectedProduct" came from Dialog
      if (result && result.event == "SelectedProduct") {
        //this.addProductToTable(netAmount, result.data);
      }
    });
  }
}
