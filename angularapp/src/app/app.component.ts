import { Component, OnInit } from '@angular/core';
import { AccountService } from './shared/pages/page-routing/account/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  
  constructor(private accountService: AccountService){

  }
  
  ngOnInit(): void {
    this.refreshUser();
  }

  private refreshUser(){
    const jwt = this.accountService.getJWT();
    const email = this.accountService.getEmail();
    if(jwt){
        this.accountService.refreshUser(jwt, email).subscribe({
          next: _ => {},
          error: _ => {
            console.log(_);
            this.accountService.logout();
          }
        })
    } else{
      this.accountService.refreshUser(null, null).subscribe();
    }
  }
}
