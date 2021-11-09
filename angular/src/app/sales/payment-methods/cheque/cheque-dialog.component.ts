import { Component, Injector, OnInit, Inject } from "@angular/core";
import {
  forEach as _forEach,
  includes as _includes,
  map as _map,
} from "lodash-es";
import { AppComponentBase } from "@shared/app-component-base";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-cheque-dialog",
  templateUrl: "./cheque-dialog.component.html",
  styleUrls: ["./cheque-dialog.component.scss"],
})
export class ChequeDialogComponent extends AppComponentBase implements OnInit {
  constructor(
    injector: Injector,
    public matDialogRef: MatDialogRef<ChequeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super(injector);
  }

  ngOnInit(): void {}
}
