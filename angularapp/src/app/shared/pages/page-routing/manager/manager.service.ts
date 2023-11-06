import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AccountService } from '../account/account.service';
import { NewRestaurant } from 'src/app/shared/models/restaurant/new-restaurant';

@Injectable({
  providedIn: 'root'
})
export class ManagerService { // add forms for newrest and updatemanager
  constructor(private http: HttpClient,
    private router: Router,
    private accountService: AccountService) { }

  getManager(email: string) {
    return this.http.get(`${environment.appUrl}/api/manager/${email}`);
  }

  deleteManagerAccount(email: string) {
    return this.http.delete(`${environment.appUrl}/api/manager/${email}`);
  }

  addNewRestaurant(model: NewRestaurant, email: string) {
    return this.http.put(`${environment.appUrl}/api/manager/${email}`, model);
  }

  updateManagerAccount(model: NewRestaurant, email: string){
    return this.http.put(`${environment.appUrl}/api/manager/${email}`, model);
  }

  logout() {
    this.accountService.logout();
  }
}
