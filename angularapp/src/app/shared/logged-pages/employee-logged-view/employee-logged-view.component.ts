import { Component, OnInit } from '@angular/core';
import { User } from '../../models/account/user';
import { Employee } from '../../models/employee/employee';
import { Restaurant } from '../../models/restaurant/restaurant';
import { AccountService } from '../../pages-routing/account/account.service';
import { EmployeeService } from '../../pages-routing/employee/employee.service';

@Component({
  selector: 'app-employee-logged-view',
  templateUrl: './employee-logged-view.component.html',
  styleUrls: ['./employee-logged-view.component.css']
})
export class EmployeeLoggedViewComponent implements OnInit {

  user: User | undefined;
  employee: Employee | undefined;

  constructor(private employeeService: EmployeeService,
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

  missingIcon(restaurant: Restaurant) {
    restaurant.iconPath = 'assets/images/logo-bw-with-bg.png'; 
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
