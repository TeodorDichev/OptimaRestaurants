import { Component } from '@angular/core';
import { AccountService } from '../../pages/page-routing/account/account.service';
import { ManagerService } from '../../pages/page-routing/manager/manager.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  constructor(public accountService: AccountService,
    public managerService: ManagerService) {}
  logout(){
      this.accountService.logout();
  }
}
