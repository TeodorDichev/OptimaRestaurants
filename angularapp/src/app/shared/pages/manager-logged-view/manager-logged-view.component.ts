import { Component, OnInit } from '@angular/core';
import { ManagerService } from '../page-routing/manager/manager.service';
import { AccountService } from '../page-routing/account/account.service';
import { User } from 'src/app/shared/models/account/user';
import { Manager } from 'src/app/shared/models/manager/manager';
import { SharedService } from '../../shared.service';
import { Restraurant } from '../../models/restaurant/restaurant';
import { Employee } from '../../models/employee/employee';


@Component({
  selector: 'app-manager-logged-view',
  templateUrl: './manager-logged-view.component.html',
  styleUrls: ['./manager-logged-view.component.css',
    '../../../app.component.css']
})
export class ManagerLoggedViewComponent implements OnInit {

  editRestaurant() {
    this.sharedService.openRestaurantEditModal(this.currentRestaurant);
  }

  user: User | null | undefined;
  manager: Manager | null | undefined;
  currentRestaurant: Restraurant | undefined;
  employees: Employee[] = [];

  constructor(private managerService: ManagerService,
    private accountService: AccountService,
    private sharedService: SharedService) { }

  addNewRestaurant() {
    this.sharedService.openRestaurantCreateModal();
  }

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

  getRestaurantEmployees() {  
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

  selectedRestaurant(selectedRestaurant: Restraurant) {
    this.currentRestaurant = selectedRestaurant;
  }

  missingIcon(restaurant: Restraurant) {
    restaurant.iconUrl = 'assets/images/logo-bw-with-bg.png'; // change logic here, this way a value is given to the iconUrl
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
        if (this.manager.restaurants){
          this.selectedRestaurant(this.manager.restaurants[0]);
          this.getRestaurantEmployees();
        }
      }
    });
  }
}
