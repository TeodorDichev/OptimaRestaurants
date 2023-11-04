import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../pages/page-routing/account/account.service';
import { ManagerService } from '../../pages/page-routing/manager/manager.service';
import { User } from '../../models/account/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  user: User | null | undefined;
  isManager: boolean = true;
  
  constructor(public accountService: AccountService,
    public managerService: ManagerService,
    public router: Router) {}
    logout(){
      this.accountService.logout();
  }
  employeeSearch(){
    
  }
  infoUser(){
    
  }
  inbox(){

  }
  contact(){

  }
  help(){

  }
  allRestaurants(){

  }
    ngOnInit() {
      this.accountService.user$.subscribe(user => {
        this.user = user;
        if (user) {
          this.isManager = user.isManager; 
        }
      });
    }
  
}
