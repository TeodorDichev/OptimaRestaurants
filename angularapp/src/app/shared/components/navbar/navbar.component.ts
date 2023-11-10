import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../pages/page-routing/account/account.service';
import { ManagerService } from '../../pages/page-routing/manager/manager.service';
import { User } from '../../models/account/user';
import { Router } from '@angular/router';
import { SharedService } from '../../shared.service';

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
    public sharedService: SharedService,
    public router: Router) { }

  ngOnInit() {
    this.accountService.user$.subscribe(user => {
      this.user = user;
      if (user) {
        this.isManager = user.isManager;
      }
    });
  }

  logout() {
    this.accountService.logout();
  }

  homePage() {
    if (this.user) {
      if (this.isManager) {
        this.router.navigateByUrl('/manager');
      }
      else {
        this.router.navigateByUrl('/employee')
      }
    }
    else {
      this.router.navigateByUrl('/')
    }
  }

  employeeSearch() {

  }
  infoUser() {
    if (this.user) {
      if (this.isManager) {
        this.sharedService.openEditManagerModal();
      }
      else {
        this.sharedService.openEditEmployeeModal();
      }
    }
  }
  inbox() {

  }
  contact() {

  }
  help() {

  }
  allRestaurants() {

  }


}
