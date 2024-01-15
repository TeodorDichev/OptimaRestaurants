import { Component, OnInit } from '@angular/core';
import { ManagerService } from '../../pages-routing/manager/manager.service';
import { AccountService } from '../../pages-routing/account/account.service';
import { User } from 'src/app/shared/models/account/user';
import { Manager } from 'src/app/shared/models/manager/manager';
import { SharedService } from '../../shared.service';
import { Restaurant } from '../../models/restaurant/restaurant';
import { Employee } from '../../models/employee/employee';
import { take } from 'rxjs';


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
    this.getUser();
    this.getManager();
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
        }
      );
    }
  }

  getEmployeeInfo(employee: Employee) {
    this.sharedService.openUserInfoModal(employee.email, 'Employee')
  }

  fireEmployee(employeeEmail: string, restaurantId: string | undefined) {
    if (restaurantId) {
      this.managerService.fireEmployee(employeeEmail, restaurantId).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, 'Успешно уволнихте служител!', 'Вече той не работи за вас.')
          this.employees = response;
        }
      })  
    }
  }

  selectedRestaurant(selectedRestaurant: Restaurant) {
    this.currentRestaurant = selectedRestaurant;
    this.getCurrrentRestaurantEmployees();
  }

  infoRestaurant() {
    if (this.currentRestaurant) {
      this.sharedService.openRestaurantDetailsModal(this.currentRestaurant.id);
    }
  }

  missingIcon(restaurant: Restaurant) {
    restaurant.iconPath = 'assets/images/logo-bw-with-bg.png';
  }

  private getUser() {
    this.accountService.user$.pipe(take(1)).subscribe({
      next: (user: any) => {
        this.user = user;
      }
    })
  }

  private getManager() {
    this.managerService.manager$.subscribe({
      next: (manager: any) => {
        this.manager = manager;
        if (this.manager?.restaurants) {
          this.selectedRestaurant(this.manager.restaurants[0]);
          this.getCurrrentRestaurantEmployees();
        }
      }
    });
  }
}
