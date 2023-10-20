import { Component } from '@angular/core';
import { AccountService } from '../../pages/account-routing/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  constructor(public accountService: AccountService) {}
  logout(){
      this.accountService.logout();
  }
}
