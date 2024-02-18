import { Component, OnDestroy, OnInit } from '@angular/core';
import { User } from '../../models/account/user';
import { Employee } from '../../models/employee/employee';
import { Restaurant } from '../../models/restaurant/restaurant';
import { AccountService } from '../../pages-routing/account/account.service';
import { EmployeeService } from '../../pages-routing/employee/employee.service';
import { SharedService } from '../../shared.service';
import { Subscription } from 'rxjs';
import { ReviewsService } from '../../pages-routing/review/reviews.service';
import { OldReview } from '../../models/reviews/old-review';

@Component({
  selector: 'app-employee-logged-view',
  templateUrl: './employee-logged-view.component.html',
  styleUrls: ['./employee-logged-view.component.css']
})
export class EmployeeLoggedViewComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  employeeReviewsHistory: OldReview[] = [];

  user: User | undefined;
  employee: Employee | undefined;

  constructor(private employeeService: EmployeeService,
    private accountService: AccountService,
    private sharedService: SharedService,
    private reviewsService: ReviewsService) { }

  ngOnInit(): void {
    this.getUser();
    this.getEmployee();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  openRestaurantInfo(restaurant: Restaurant) {
    this.sharedService.openRestaurantDetailsModal(restaurant.id);
  }

  missingIcon(restaurant: Restaurant) {
    restaurant.iconPath = 'assets/images/logo-bw-with-bg.png';
  }

  private getUser() {
    const sub = this.accountService.user$.subscribe(user => {
      if (user) {
        this.user = user;
      }
    });
    this.subscriptions.push(sub);
  }

  private getEmployee() {
    const sub = this.employeeService.employee$.subscribe(employee => {
      if (employee) {
        this.employee = employee;
      }
    });
    this.subscriptions.push(sub);
  }

  getEmployeeLastRequests() {
    if (this.employee) {
      const sub = this.reviewsService.getEmployeeReviewsHistory(this.employee.email).subscribe({
        next: (response: any) => {
          console.log(response);
          this.employeeReviewsHistory = response;
        }
      });
      this.subscriptions.push(sub);
    }
  }
}
