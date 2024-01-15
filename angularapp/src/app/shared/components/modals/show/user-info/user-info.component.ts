import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';
import { Employee } from 'src/app/shared/models/employee/employee';
import { Manager } from 'src/app/shared/models/manager/manager';
import { EmployeeRequest } from 'src/app/shared/models/requests/employeeRequest';
import { Restaurant } from 'src/app/shared/models/restaurant/restaurant';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent implements OnInit {
  @Input() email: string | undefined;
  @Input() role: string | undefined;

  employee: Employee | undefined;
  manager: Manager | undefined;
  currentUser: User | undefined;

  currentManagersRestaurants: Restaurant[] = [];
  selectedRestaurant: Restaurant | undefined;
  workingRequest: EmployeeRequest = {
    restaurantId: '',
    employeeEmail: ''
  };

  constructor(private employeeService: EmployeeService,
    private managerService: ManagerService,
    private accountService: AccountService,
    private sharedService: SharedService,
    private bsModalRef: BsModalRef) { }

  ngOnInit() {
    this.getUser();
  }

  getUser() {

    this.accountService.user$.pipe(take(1)).subscribe({
      next: (response: any) => {
        this.currentUser = response;
        if (this.currentUser?.isManager)
          this.takeCurrentManagerRestaurants();
      }
    })

    if (this.email) {
      if (this.role == 'Employee') {
        this.employeeService.getEmployee(this.email).pipe(take(1)).subscribe({
          next: (response: any) => {
            this.employee = response;
          }
        })
      }

      else if (this.role == 'Manager') {
        this.managerService.getManager(this.email).pipe(take(1)).subscribe({
          next: (response: any) => {
            this.manager = response;
          }
        })
      }
    }

  }

  takeCurrentManagerRestaurants() {
    if (this.currentUser?.email) {
      this.managerService.getManager(this.currentUser.email).pipe(take(1)).subscribe({
        next: (response: any) => {
          this.currentManagersRestaurants = response.restaurants;
        }
      })
    }
  }

  selectRestaurant(rest: Restaurant) {
    this.selectedRestaurant = rest;
  }

  sendWorkingRequest(restaurantId: string) {
    if (this.email){
      this.workingRequest.employeeEmail = this.email;
      this.workingRequest.restaurantId = restaurantId;
      this.managerService.employeeWorkingRequest(this.workingRequest).pipe(take(1)).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.bsModalRef.hide();
        }
      })
    }
  }

  // this checks if the current user, that's a manager, has the employee he searches for as a employee for some of his restaurants
  // if true he can be left reviews
  employeeWorksForManager() {
    if (this.currentUser?.isManager && this.employee) {
      // logik
      return true;
    }
    return false;
  }
}
