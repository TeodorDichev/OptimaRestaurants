import { Component, OnInit } from '@angular/core';
import { AccountService } from './shared/pages-routing/account/account.service';
import { EmployeeService } from './shared/pages-routing/employee/employee.service';
import { ManagerService } from './shared/pages-routing/manager/manager.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(private accountService: AccountService,
    private employeeService: EmployeeService,
    private managerService: ManagerService) { }

  ngOnInit(): void {
    this.refreshUser();
  }

  private refreshUser() {
    const jwt = this.accountService.getJWT();
    const email = this.accountService.getEmail();
    const isManager = this.accountService.getIsManager();
    if (jwt && email) {
      this.accountService.refreshUser(jwt, email).subscribe({
        next: _ => {
          if (isManager)
            this.managerService.refreshManager(email);
          else if (!isManager) {
            this.employeeService.refreshEmployee(email);
          }
        },
        error: _ => {
          // console.log(_);
          this.accountService.logout();
        }
      })
    } else {
      this.accountService.refreshUser(null, null).subscribe();
    }
  }
}
