import { Injectable } from '@angular/core';
import { Router, Routes } from '@angular/router';
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
    const currentPage = currentUrl.split('/').at(3);
    return this.accountService.user$.pipe(
      map((user: User | null) => {
        if (user) {
          if (user.isManager) {
            if (currentPage === 'employee') {
              this.incorrect('manager');
              return false;
            }
            return true;
          }
          else {
            if (currentPage === 'manager') {
              this.incorrect('employee')
              return false;
            }
            return true;
          }
        }
        else {
          this.incorrect('');
          return false;
        }
      })
    );
  }
  private incorrect(page: string) {
    this.sharedService.showNotification(false, 'Not authorized', 'You are either not logged in, or do not have access to this page.');
    this.router.navigateByUrl('/' + page);
  }

}   
