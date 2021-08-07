import { Component } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { LoaderService } from '../loader/loader.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss'],
})
export class NavComponent {
  isDarkTheme: boolean = true;
  isHandset$: Observable<boolean> = this.breakpointObserver
    .observe(Breakpoints.Handset)
    .pipe(
      map((result) => result.matches),
      shareReplay()
    );

  constructor(
    private breakpointObserver: BreakpointObserver,
    public loaderService: LoaderService
  ) {}

  ngOnInit() {
    this.isDarkTheme = localStorage.getItem('theme') === 'Dark' ? true : false;
  }

  storeThemeSelection() {
    localStorage.setItem('theme', this.isDarkTheme ? 'Dark' : 'Light');
  }
}
