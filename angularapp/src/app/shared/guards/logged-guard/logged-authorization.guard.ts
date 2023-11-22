import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../../pages/account/account.service';
import { SharedService } from '../../shared.service';
import { Observable, map } from 'rxjs';
import { User } from '../../models/account/user';


@Injectable({
  providedIn: 'root'
})

export class LoggedAuthorizationGuard {
  constructor(private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router) { }

  canActivate(): Observable<boolean> {
    return this.accountService.user$.pipe(
      map((user: User | null) => {   
        if (user) {
          if(user.isManager) {
          this.incorrect('manager');
          return false;
          }
          this.incorrect('employee');
          return false;
        }      
        return true;
      })
    );
  }


  private incorrect(page: string) {
    this.sharedService.showNotification(false, 'Not authorized', 'You are either not logged in, or do not have access to this page.');
    this.router.navigateByUrl('/' + page);
  }

}   
