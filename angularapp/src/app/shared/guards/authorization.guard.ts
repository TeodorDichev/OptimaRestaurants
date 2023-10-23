import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { AccountService } from '../pages/account-routing/account.service';
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

  canActivate(route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> {
    return this.accountService.user$.pipe(
      map((user: User | null) => {
        if (user) {
          return true;
        }
        else {
          this.sharedService.showNotification(false, 'Not authorized', 'This page requires a login. You will be redirected now.');
          // this.router.navigate(commands) 3:28:37 --> redirect to index page prob.
          return false;
        }
      })
    );
  }
}   
