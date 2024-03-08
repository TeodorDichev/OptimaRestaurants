import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../../pages-routing/account/account.service';
import { Observable, map } from 'rxjs';
import { User } from '../../models/account/user';


@Injectable({
  providedIn: 'root'
})

export class LoggedAuthorizationGuard {
  constructor(private accountService: AccountService,
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
    this.router.navigateByUrl('/' + page);
  }
}   
