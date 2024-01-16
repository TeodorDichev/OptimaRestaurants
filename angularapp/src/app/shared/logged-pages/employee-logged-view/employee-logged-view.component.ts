import { Component, OnInit } from '@angular/core';
import { User } from '../../models/account/user';
import { Employee } from '../../models/employee/employee';
import { Restaurant } from '../../models/restaurant/restaurant';
import { AccountService } from '../../pages-routing/account/account.service';
import { EmployeeService } from '../../pages-routing/employee/employee.service';
import { SharedService } from '../../shared.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-employee-logged-view',
  templateUrl: './employee-logged-view.component.html',
  styleUrls: ['./employee-logged-view.component.css']
})
export class EmployeeLoggedViewComponent implements OnInit {

  user: User | undefined;
  employee: Employee | undefined;

  constructor(private employeeService: EmployeeService,
    private accountService: AccountService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getUser();
    this.getEmployee();
  }

  openRestaurantInfo(restaurant: Restaurant) {
    this.sharedService.openRestaurantDetailsModal(restaurant.id);
  }

  missingIcon(restaurant: Restaurant) {
    restaurant.iconPath = 'assets/images/logo-bw-with-bg.png'; 
  }

  private getUser() {
    this.accountService.user$.pipe(take(1)).subscribe(user => {
      if (user) {
        this.user = user;
      }
    });
  }

  private getEmployee() {
    this.employeeService.employee$.subscribe(employee => {
      if (employee) {
        this.employee = employee;
      }
    });
  }
}
