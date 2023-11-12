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

  getManager(email: string) { // works
    return this.http.get(`${environment.appUrl}/api/manager/get-manager/${email}`);
  }

  deleteManagerAccount(email: string) {  // works
    this.logout();
    return this.http.delete(`${environment.appUrl}/api/manager/delete-manager/${email}`);
  }

  addNewRestaurant(model: NewRestaurant, email: string) { // works
    return this.http.post(`${environment.appUrl}/api/manager/add-new-restaurant/${email}`, model);
  }

  updateManagerAccount(model: UpdateManager, email: string){ // works
    return this.http.put(`${environment.appUrl}/api/manager/update-manager/${email}`, model);
  }

  getRestaurantEmployees(restaurantId: string) { // not possible yet, in the making...
    return this.http.get(`${environment.appUrl}/api/manager/get-restaurant-employees/${restaurantId}`);
  }

  editRestaurant(model: NewRestaurant, restaurantId: string) { // works
    return this.http.put(`${environment.appUrl}/api/manager/update-restaurant/${restaurantId}`, model);
  }

  logout() { // works
    this.userSource.next(null); // to ensure we remove the logged MANAGER NOT USER from HERE NOT LOCAL STORAGE
    this.accountService.logout();
  }
  
  setManager(manager: Manager){ // works
    this.userSource.next(manager);
  }
}
