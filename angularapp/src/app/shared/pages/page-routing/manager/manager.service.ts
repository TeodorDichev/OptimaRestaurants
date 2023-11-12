import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { AccountService } from '../account/account.service';
import { NewRestaurant } from 'src/app/shared/models/restaurant/new-restaurant';
import { UpdateManager } from 'src/app/shared/models/manager/update-manager';
import { ReplaySubject } from 'rxjs';
import { Manager } from 'src/app/shared/models/manager/manager';

@Injectable({
  providedIn: 'root'
})
export class ManagerService { 

  private userSource = new ReplaySubject<Manager | null>(1); 
  manager$ = this.userSource.asObservable(); 

  constructor(private http: HttpClient,
    private accountService: AccountService) { }

  getManager(email: string) {
    return this.http.get(`${environment.appUrl}/api/manager/${email}`);
  }

  deleteManagerAccount(email: string) {
    return this.http.delete(`${environment.appUrl}/api/manager/${email}`);
  }

  addNewRestaurant(model: NewRestaurant, email: string) {
    return this.http.post(`${environment.appUrl}/api/manager/add-new-restaurant/${email}`, model);
  }

  updateManagerAccount(model: UpdateManager, email: string){
    return this.http.put(`${environment.appUrl}/api/manager/${email}`, model);
  }

  getRestaurantEmployees(restaurantId: string) {
    return this.http.get(`${environment.appUrl}/api/manager/get-restaurant-employees/${restaurantId}`);
  }

  editRestaurant(model: NewRestaurant, restaurantId: string) {
    return this.http.put(`${environment.appUrl}/api/manager/update-restaurant/${restaurantId}`, model);
  }

  logout() {
    this.accountService.logout();
  }
  
  setManager(manager: Manager){
    this.userSource.next(manager);
  }
}
