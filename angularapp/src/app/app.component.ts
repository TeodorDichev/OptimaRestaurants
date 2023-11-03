import { Component, OnInit } from '@angular/core';
import { AccountService } from './shared/pages/page-routing/account/account.service';
import { ManagerService } from './shared/pages/page-routing/manager/manager.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  
  constructor(private accountService: AccountService,
    private managerService: ManagerService){

  }
  
  ngOnInit(): void {
    this.refreshUser();
  }

  private refreshUser(){
    const jwt = this.accountService.getJWT();
    if(jwt){
        this.accountService.refreshUser(jwt).subscribe({
          next: _ => {},
          error: _ => {
            console.log(jwt);
            console.log(_);
            this.accountService.logout();
          }
        })
    } else{
      this.accountService.refreshUser(null).subscribe();
    }
  }
}
