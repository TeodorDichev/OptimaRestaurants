import { Component } from '@angular/core';
import { AccountService } from '../../pages/page-routing/account/account.service';
import { ManagerService } from '../../pages/page-routing/manager/manager.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  isManager: boolean = true; //= this.accountService.user$.jwt;
  
  constructor(public accountService: AccountService,
    public managerService: ManagerService) {}
  logout(){
      this.accountService.logout();
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
}
