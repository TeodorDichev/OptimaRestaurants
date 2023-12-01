import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/shared/models/account/user';
import { EmployeeRequest } from 'src/app/shared/models/requests/employeeRequest';
import { Restaurant } from 'src/app/shared/models/restaurant/restaurant';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
import { RestaurantsService } from 'src/app/shared/pages-routing/restaurants/restaurants.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-restaurant-info',
  templateUrl: './restaurant-info.component.html',
  styleUrls: ['./restaurant-info.component.css']
})
export class RestaurantInfoComponent implements OnInit {
  @Input() restaurant: Restaurant | undefined;
  user: User | undefined;
  employeeRequest: EmployeeRequest = {
    restaurantId: '',
    employeeEmail: ''
  };

  constructor(public bsModalRef: BsModalRef,
    private accountService: AccountService,
    private restaurantsService: RestaurantsService,
    private sharedService: SharedService) { }

  ngOnInit() {
    this.getUserInfo();
  }

  close() {
    this.bsModalRef.hide();
  }

  getUserInfo() {
    this.accountService.user$.subscribe({
      next: (response: any) => {
        this.user = response;
      }
    })
  }

  sendWorkingRequest() {
    if (this.restaurant && this.user) {
      this.employeeRequest.restaurantId = this.restaurant.id;
      this.employeeRequest.employeeEmail = this.user.email;
      this.restaurantsService.sendWorkingRequest(this.employeeRequest).subscribe({
        next: (response: any) =>{
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.bsModalRef.hide();
        },
        error: error => {
          this.sharedService.showNotification(false, error.value.title, error.value.message);
        }
      })
    }
  }
}
