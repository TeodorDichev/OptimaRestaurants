import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../pages-routing/account/account.service';
import { ManagerService } from '../../pages-routing/manager/manager.service';
import { User } from '../../models/account/user';
import { Router } from '@angular/router';
import { SharedService } from '../../shared.service';
import { EmployeeService } from '../../pages-routing/employee/employee.service';

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
    public employeeService: EmployeeService,
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
    if (this.user?.isManager){
      this.managerService.logout();
    } else {
      this.employeeService.logout();
    }
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

  showCV() {
    this.sharedService.openCVModal();
  }

  showQR() {
    this.sharedService.openQRCodeModal();
  }

  employeeSearch() {
    
  }

  infoUser() {
    if (this.user) {
      if (this.isManager) {
        this.sharedService.openManagerInfoModal();
      }
      else {
        this.sharedService.openEmployeeInfoModal();
      }
    }
  }

  inbox() {
    if (this.user) {
      if (this.isManager) {
        this.sharedService.openInboxModalManager();
      }
      else {
        this.sharedService.openInboxModalEmployee();
      }
    }
  }

  contact() {

  }

  help() {

  }

  allRestaurants() {
    this.router.navigateByUrl('/restaurants');
  }


}
