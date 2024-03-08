import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';
import { Employee } from 'src/app/shared/models/employee/employee';
import { Manager } from 'src/app/shared/models/manager/manager';
import { EmployeeRequest } from 'src/app/shared/models/requests/employeeRequest';
import { Restaurant } from 'src/app/shared/models/restaurant/restaurant';
import { OldReview } from 'src/app/shared/models/reviews/old-review';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { ReviewsService } from 'src/app/shared/pages-routing/review/reviews.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  @Input() email: string | undefined;
  @Input() role: string | undefined;

  employee: Employee | undefined;
  manager: Manager | undefined;
  currentUser: User | undefined;

  employeeReviewsHistory: OldReview[] = [];

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
    private bsModalRef: BsModalRef,
    private reviewsService: ReviewsService) { }

  ngOnInit() {
    this.getUser();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  getUser() {
    const sub1 = this.accountService.user$.subscribe({
      next: (response: any) => {
        this.currentUser = response;
        if (this.currentUser?.isManager)
          this.takeCurrentManagerRestaurants();
      }
    });
    this.subscriptions.push(sub1);

    if (this.email) {
      if (this.role == 'Employee') {
        const sub = this.employeeService.getEmployee(this.email).subscribe({
          next: (response: any) => {
            this.employee = response;
          }
        });
        this.subscriptions.push(sub);
      }
      else if (this.role == 'Manager') {
        const sub = this.managerService.getManager(this.email).subscribe({
          next: (response: any) => {
            this.manager = response;
          }
        });
        this.subscriptions.push(sub);
      }
    }
  }

  takeCurrentManagerRestaurants() {
    if (this.currentUser?.email) {
      const sub = this.managerService.getManager(this.currentUser.email).subscribe({
        next: (response: any) => {
          this.currentManagersRestaurants = response.restaurants;
        }
      });
      this.subscriptions.push(sub);
    }
  }

  selectRestaurant(rest: Restaurant) {
    this.selectedRestaurant = rest;
  }

  sendWorkingRequest(restaurantId: string) {
    if (this.email) {
      this.workingRequest.employeeEmail = this.email;
      this.workingRequest.restaurantId = restaurantId;
      const sub = this.managerService.employeeWorkingRequest(this.workingRequest).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.bsModalRef.hide();
        },
        error: error => {
          this.sharedService.showNotification(false, 'Неуспешно изпращане на заявка!', error.error);
        }
      });
      this.subscriptions.push(sub);
    }
  }

  getEmployeeLastRequests() {
    if (this.employee) {
      const sub = this.reviewsService.getEmployeeReviewsHistory(this.employee.email).subscribe({
        next: (response: any) => {
          this.employeeReviewsHistory = response;
        }
      });
      this.subscriptions.push(sub);
    }
  }

  missingIconEmployee(employee: Employee) {
    //employee.profilePicturePath = 'assets/images/logo-bw-with-bg.png';
  }

  missingIconManager(manager: Manager) {
    //manager.profilePicturePath = 'assets/images/logo-bw-with-bg.png';
  }
}
