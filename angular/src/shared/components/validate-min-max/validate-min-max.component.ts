import { Component, Input, OnInit } from "@angular/core";
import { AbstractControl } from "@angular/forms";

@Component({
  selector: "validate-min-max",
  templateUrl: "./validate-min-max.component.html",
  styleUrls: ["./validate-min-max.component.scss"],
})
export class ValidateMinMaxComponent implements OnInit {
  @Input() control: AbstractControl;
  @Input() minValue: number | null;
  @Input() maxValue: number | null;
  @Input() enteredValue: number | null;

  ngOnInit(): void {}
}
