import { Component, OnDestroy, OnInit } from '@angular/core';
import { User } from 'src/app/shared/models/account/user';
import { Manager } from 'src/app/shared/models/manager/manager';
import { Employee } from '../../models/employee/employee';
import { Restaurant } from '../../models/restaurant/restaurant';
import { AccountService } from '../../pages-routing/account/account.service';
import { ManagerService } from '../../pages-routing/manager/manager.service';
import { SharedService } from '../../shared.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-manager-logged-view',
  templateUrl: './manager-logged-view.component.html',
  styleUrls: ['./manager-logged-view.component.css']
})
export class ManagerLoggedViewComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

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

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  addNewRestaurant() {
    this.sharedService.openRestaurantCreateModal();
  }

  editRestaurant() {
    this.sharedService.openRestaurantEditModal(this.currentRestaurant);
  }

  getCurrrentRestaurantEmployees() {
    if (this.currentRestaurant?.id) {
      const sub = this.managerService.getRestaurantEmployees(this.currentRestaurant.id).subscribe(
        (response: any) => {
          this.employees = response;
        });
      this.subscriptions.push(sub);
    }
  }

  getEmployeeInfo(employee: Employee) {
    this.sharedService.openUserInfoModal(employee.email, 'Employee')
  }

  fireEmployee(employeeEmail: string, restaurantId: string) {
    const sub = this.managerService.fireEmployee(employeeEmail, restaurantId).subscribe({
      next: (response: any) => {
        this.sharedService.showNotification(true, 'Успешно уволнихте служител!', 'Вече той не работи за вас.')
        this.employees = response;
      }
    });
    this.subscriptions.push(sub);
  }

  openManagerReviewModal(employeeEmail: string, restaurantId: string) { 
    this.sharedService.openManagerReviewModal(employeeEmail, restaurantId);
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
    const sub = this.accountService.user$.subscribe({
      next: (user: any) => {
        this.user = user;
      }
    });
    this.subscriptions.push(sub);
  }

  private getManager() {
    const sub = this.managerService.manager$.subscribe({
      next: (manager: any) => {
        this.manager = manager;
        if (this.manager?.restaurants) {
          this.selectedRestaurant(this.manager.restaurants[0]);
          this.getCurrrentRestaurantEmployees();
        }
      }
    });
    this.subscriptions.push(sub);
  }
}
