import { Component, OnInit } from '@angular/core';
import { ManagerService } from '../../pages/manager/manager.service';
import { AccountService } from '../../pages/account/account.service';
import { User } from 'src/app/shared/models/account/user';
import { Manager } from 'src/app/shared/models/manager/manager';
import { SharedService } from '../../shared.service';
import { Restaurant } from '../../models/restaurant/restaurant';
import { Employee } from '../../models/employee/employee';


@Component({
  selector: 'app-manager-logged-view',
  templateUrl: './manager-logged-view.component.html',
  styleUrls: ['./manager-logged-view.component.css']
})
export class ManagerLoggedViewComponent implements OnInit {

  user: User | null | undefined;
  manager: Manager | null | undefined;
  currentRestaurant: Restaurant | undefined;
  employees: Employee[] = [];

  constructor(private managerService: ManagerService,
    private accountService: AccountService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.setUser();
    if (this.user) {
      this.managerService.getManager(this.user.email).subscribe({
        next: (response: any) => {
          this.managerService.setManager(response);
          this.setManager();
        }
      });
    }
  }

  addNewRestaurant() {
    this.sharedService.openRestaurantCreateModal();
  }

  editRestaurant() {
    this.sharedService.openRestaurantEditModal(this.currentRestaurant);
  }

  getCurrrentRestaurantEmployees() {
    if (this.currentRestaurant?.id) {
      this.managerService.getRestaurantEmployees(this.currentRestaurant.id).subscribe(
        (response: any) => {
          this.employees = response;
        },
        (error) => {
          console.log(error.error);
        }
      );
    }
  }

  selectedRestaurant(selectedRestaurant: Restaurant) {
    this.currentRestaurant = selectedRestaurant;
    this.getCurrrentRestaurantEmployees();
  }

  missingIcon(restaurant: Restaurant) {
    restaurant.iconUrl = 'assets/images/logo-bw-with-bg.png'; 
  }

  private setUser() {
    this.accountService.user$.subscribe(user => {
      if (user) {
        this.user = user;
      }
    });
  }
  private setManager() {
    this.managerService.manager$.subscribe(manager => {
      if (manager) {
        this.manager = manager;
        if (this.manager.restaurants) {
          this.selectedRestaurant(this.manager.restaurants[0]);
          this.getCurrrentRestaurantEmployees();
        }
      }
    });
  }
}
