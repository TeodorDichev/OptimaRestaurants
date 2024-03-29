import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';
import { EmployeeRequest } from 'src/app/shared/models/requests/employeeRequest';
import { Restaurant } from 'src/app/shared/models/restaurant/restaurant';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';
import { RestaurantsService } from 'src/app/shared/pages-routing/restaurants/restaurants.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-restaurant-info',
  templateUrl: './restaurant-info.component.html',
  styleUrls: ['./restaurant-info.component.css']
})
export class RestaurantInfoComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  @Input() restaurantId!: string;

  employeeWorksInRestaurant: boolean | undefined;
  managerOwnsRestaurant: boolean | undefined;
  restaurant: Restaurant | undefined;
  user: User | undefined;

  employeeRequest: EmployeeRequest = {
    restaurantId: '',
    employeeEmail: ''
  };

  constructor(public bsModalRef: BsModalRef,
    private accountService: AccountService,
    private restaurantsService: RestaurantsService,
    private employeeService: EmployeeService,
    private sharedService: SharedService) { }

  ngOnInit() {
    this.getUserInfo();
    this.getRestaurant();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  close() {
    this.bsModalRef.hide();
  }

  getRestaurant() {
    const sub = this.restaurantsService.getRestaurantDetails(this.restaurantId).subscribe({
      next: (response: any) => {
        this.restaurant = response;
        if (!!!this.user?.isManager) {
          this.checkIfEmployeeWorksInRestaurant();
        }
        else {
          this.checkIfManagerOwnsRestaurant();
        }
      }
    });
    this.subscriptions.push(sub);
  }

  getUserInfo() {
    const sub = this.accountService.user$.subscribe({
      next: (response: any) => {
        this.user = response;
      }
    });
    this.subscriptions.push(sub);
  }

  checkIfEmployeeWorksInRestaurant() {
    const sub = this.employeeService.employee$.subscribe({
      next: (response: any) => {
        this.employeeWorksInRestaurant = response.restaurants.some((item: { id: string; }) => item.id === this.restaurantId);
      }
    });
    this.subscriptions.push(sub);
  }

  checkIfManagerOwnsRestaurant() {
    if (this.user?.email == this.restaurant?.managerEmail) {
      this.managerOwnsRestaurant = true;
    }
  }

  sendWorkingRequest() {
    if (this.restaurant && this.user) {
      this.employeeRequest.restaurantId = this.restaurant.id;
      this.employeeRequest.employeeEmail = this.user.email;
      const sub = this.restaurantsService.sendWorkingRequest(this.employeeRequest).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.bsModalRef.hide();
        },
        error: error => {
          this.sharedService.showNotification(false, 'Неуспешно изпращане!', error.error);
        }
      });
      this.subscriptions.push(sub);
    }
  }

  openTopEmployeeInfo(restaurantTopEmployeeEmail: string) {
    this.sharedService.openUserInfoModal(restaurantTopEmployeeEmail, 'Employee');
    this.bsModalRef.hide();
  }

  openManagerInfo(managerEmail: string) {
    this.sharedService.openUserInfoModal(managerEmail, 'Manager');
    this.bsModalRef.hide();
  }

  missingIcon(restaurant: Restaurant) {
    restaurant.iconPath = 'assets/images/logo-bw-with-bg.png';
  }

  missingIconEmployee(restaurant: Restaurant) {
    restaurant.topEmployeePicturePath = 'assets/images/logo-bw-with-bg.png';
  }
}
