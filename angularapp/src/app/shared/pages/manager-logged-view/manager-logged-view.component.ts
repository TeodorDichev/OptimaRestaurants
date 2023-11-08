import { Component, OnInit } from '@angular/core';
import { ManagerService } from '../page-routing/manager/manager.service';
import { Router } from '@angular/router';
import { AccountService } from '../page-routing/account/account.service';
import { User } from 'src/app/shared/models/account/user';
import { ManagerView } from 'src/app/shared/models/manager/manager-view';
import { SharedService } from '../../shared.service';

@Component({
  selector: 'app-manager-logged-view',
  templateUrl: './manager-logged-view.component.html',
  styleUrls: ['./manager-logged-view.component.css',
  '../../../app.component.css']
})
export class ManagerLoggedViewComponent implements OnInit {
  user: User | null | undefined;
  
  manager: ManagerView = {
    email: '',
    firstName: '',
    lastName: '',
    profilePictureUrl: '',
    restaurants: []
  };

  constructor(private managerService: ManagerService,
    private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router){

  }

  ngOnInit(): void {
    this.setManager();
    if (this.user){
    this.managerService.getManager(this.user.email).subscribe(
      (response: any) => {
        this.manager = response;
      },
      (error) => {      
        console.log(error.error);
      }
    );;
    }
  }
  private setManager(){
    this.accountService.user$.subscribe(user => {
      if (user) {
        this.user = user;
      }
      });
  }

  addNewRestaurant(){
    this.sharedService.showRestaurantModal();
  }
   
  getRestaurantEmployees() {
    this.managerService.getRestaurantEmployees('restaurantId - CHANGE WITH WHAT IS NEEDED').subscribe(
      (response: any) => {
        // this.employees (which needs employee dto []) = response;
      },
      (error) => {      
        console.log(error.error);
      }
    );
  }

}
