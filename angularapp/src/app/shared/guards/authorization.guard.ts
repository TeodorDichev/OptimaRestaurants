import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../pages/page-routing/account/account.service';
import { SharedService } from '../shared.service';
import { Observable, map } from 'rxjs';
import { User } from '../models/account/user';


@Injectable({
  providedIn: 'root'
})

export class AuthorizationGuard {
  constructor(private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router) { }



  canActivate(): Observable<boolean> {
    const currentUrl = window.location.href;

    return this.accountService.user$.pipe(
      map((user: User | null) => {
        if (user) {
          if (user.isManager) {
            if (currentUrl.split('/').at(3) === 'employee') {
              this.incorrect()
              return false;
            }
            return true;
          }
          else {
            if (currentUrl.split('/').at(3) === 'manager') {
              this.incorrect()
              return false;
            }
            return true;
          }
        }
        else {
          this.incorrect()
          return false;
        }
      })
    );
  }
  private incorrect() {
    this.sharedService.showNotification(false, 'Not authorized', 'This page requires a login. Please, login first.');
    this.router.navigateByUrl('/');
  }
}   
