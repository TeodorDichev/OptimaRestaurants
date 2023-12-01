import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { AccountService } from '../account/account.service';
import { NewRestaurant } from 'src/app/shared/models/restaurant/new-restaurant';
import { UpdateManager } from 'src/app/shared/models/manager/update-manager';
import { ReplaySubject } from 'rxjs';
import { Manager } from 'src/app/shared/models/manager/manager';
import { RequestResponse } from '../../models/requests/requestResponse';

@Injectable({
  providedIn: 'root'
})
export class ManagerService {

  private userSource = new ReplaySubject<Manager | null>(1);
  manager$ = this.userSource.asObservable();

  constructor(private http: HttpClient,
    private accountService: AccountService) { }

  getManager(email: string) {
    return this.http.get(`${environment.appUrl}/api/manager/get-manager/${email}`);
  }

  deleteManagerAccount(email: string) {
    this.logout();
    return this.http.delete(`${environment.appUrl}/api/manager/delete-manager/${email}`);
  }

  addNewRestaurant(model: NewRestaurant, email: string) {
    const formData: FormData = new FormData();

    formData.append('name', model.name);
    formData.append('address', model.address);
    formData.append('city', model.city);
    formData.append('employeeCapacity', model.employeeCapacity.toString());
    formData.append('iconFile', model.iconFile);

    return this.http.post(`${environment.appUrl}/api/manager/add-new-restaurant/${email}`, formData);
  }

  getRequests(email: string) {
    return this.http.get(`${environment.appUrl}/api/manager/get-all-requests/${email}`);
  }

  respondToRequest(requestResponse: RequestResponse) {
    return this.http.post(`${environment.appUrl}/api/manager/respond-to-request`, requestResponse);
  }

  updateManagerAccount(model: UpdateManager, email: string) {

    const formData: FormData = new FormData(); // for possible file sending, otherwise I send a link to the image (only way i found) //

    formData.append('newFirstName', model.newFirstName);
    formData.append('newLastName', model.newLastName);
    formData.append('newPhoneNumber', model.newPhoneNumber);
    formData.append('profilePictureFile', model.profilePictureFile);

    return this.http.put(`${environment.appUrl}/api/manager/update-manager/${email}`, formData);
  }

  getRestaurantEmployees(restaurantId: string) {
    return this.http.get(`${environment.appUrl}/api/manager/get-restaurant-employees/${restaurantId}`);
  }

  editRestaurant(model: NewRestaurant, restaurantId: string) {
    const formData: FormData = new FormData();

    formData.append('name', model.name);
    formData.append('address', model.address);
    formData.append('city', model.city);
    formData.append('employeeCapacity', model.employeeCapacity.toString());
    formData.append('iconFile', model.iconFile);

    return this.http.put(`${environment.appUrl}/api/manager/update-restaurant/${restaurantId}`, formData);
  }

  logout() {
    this.userSource.next(null); // to ensure we remove the logged MANAGER {NOT USER} from HERE {NOT LOCAL STORAGE}
    this.accountService.logout();
  }

  setManager(manager: Manager) {
    this.userSource.next(manager);
  }
}
