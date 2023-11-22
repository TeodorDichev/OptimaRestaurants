import { Component, OnInit } from '@angular/core';
import { Employee } from '../../models/employee/employee';
import { Restraurant } from '../../models/restaurant/restaurant';
import { SharedService } from '../../shared.service';
import { EmployeeService } from '../../pages/employee/employee.service';
import { AccountService } from '../../pages/account/account.service';
import { User } from '../../models/account/user';

@Component({
  selector: 'app-employee-logged-view',
  templateUrl: './employee-logged-view.component.html',
  styleUrls: ['./employee-logged-view.component.css',
    '../../../app.component.css']
})
export class EmployeeLoggedViewComponent implements OnInit {

  user: User | undefined;
  employee: Employee | undefined;

  constructor(private sharedService: SharedService,
    private employeeService: EmployeeService,
    private accountService: AccountService) { }

  ngOnInit(): void {
    this.setUser();
    if (this.user) {
      this.employeeService.getEmployee(this.user.email).subscribe({
        next: (response: any) => {
          this.employeeService.setEmployee(response);
          this.setEmployee();
        }
      });
    }
  }

  missingIcon(restaurant: Restraurant) {
    restaurant.iconUrl = 'assets/images/logo-bw-with-bg.png'; // change logic here, this way a value is given to the iconUrl
  }

  private setUser() {
    this.accountService.user$.subscribe(user => {
      if (user) {
        this.user = user;
      }
    });
  }

  private setEmployee() {
    this.employeeService.employee$.subscribe(employee => {
      if (employee) {
        this.employee = employee;
      }
    });
  }
}
