import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { AccountService } from '../account/account.service';
import { NewRestaurant } from 'src/app/shared/models/restaurant/new-restaurant';
import { UpdateManager } from 'src/app/shared/models/manager/update-manager';
import { ReplaySubject } from 'rxjs';
import { Manager } from 'src/app/shared/models/manager/manager';
import { RequestResponse } from '../../models/requests/requestResponse';
import { EmployeeRequest } from '../../models/requests/employeeRequest';
import { CreateScheduleAssignment } from '../../models/employee/create-schedule-assignent';
import { ScheduleAssignment } from '../../models/employee/schedule-assignment';

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

  fireEmployee(employeeEmail: string, restaurantId: string) {
    return this.http.put(`${environment.appUrl}/api/manager/fire/${employeeEmail}/${restaurantId}`, {});
  }

  addNewRestaurant(model: NewRestaurant, email: string) {
    const formData: FormData = new FormData();

    formData.append('name', model.name);
    formData.append('address1', model.address1);
    formData.append('address2', model.address2);
    formData.append('city', model.city);
    formData.append('employeeCapacity', model.employeeCapacity.toString());
    formData.append('iconFile', model.iconFile);
    formData.append('longitude', model.longitude.toString());
    formData.append('latitude', model.latitude.toString());

    return this.http.post(`${environment.appUrl}/api/manager/add-new-restaurant/${email}`, formData);
  }

  getRequests(email: string) {
    return this.http.get(`${environment.appUrl}/api/manager/get-all-requests/${email}`);
  }

  employeeWorkingRequest(employeeRequest: EmployeeRequest) {
    return this.http.post(`${environment.appUrl}/api/manager/send-working-request`, employeeRequest);
  }

  getCountOfEmployeesLookingForJob() {
    return this.http.get(`${environment.appUrl}/api/manager/browse-employees/looking-for-job-count`);
  }

  getEmployeesLookingForJob(pageIndex: number) {
    return this.http.get(`${environment.appUrl}/api/manager/browse-employees/looking-for-job/${pageIndex}`);
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
    formData.append('oldPassword', model.oldPassword);
    formData.append('newPassword', model.newPassword);

    return this.http.put(`${environment.appUrl}/api/manager/update-manager/${email}`, formData);
  }

  getRestaurantEmployees(restaurantId: string) {
    return this.http.get(`${environment.appUrl}/api/manager/get-restaurant-employees/${restaurantId}`);
  }

  editRestaurant(model: NewRestaurant, restaurantId: string) {
    const formData: FormData = new FormData();

    formData.append('name', model.name);
    formData.append('address1', model.address1);
    formData.append('address2', model.address2);
    formData.append('city', model.city);
    formData.append('employeeCapacity', model.employeeCapacity.toString());
    formData.append('iconFile', model.iconFile);
    formData.append('longitude', model.longitude.toString());
    formData.append('latitude', model.latitude.toString());

    return this.http.put(`${environment.appUrl}/api/manager/update-restaurant/${restaurantId}`, formData);
  }

  deleteRestaurant(restaurantId: string) {
    return this.http.delete(`${environment.appUrl}/api/manager/delete-restaurant/${restaurantId}`)
  }

  addAssignment(createScheduleAssignment: CreateScheduleAssignment) {
    return this.http.post(`${environment.appUrl}/api/manager/schedule/add-assignment`, createScheduleAssignment);
  }

  editAssignment(scheduleAssignment: ScheduleAssignment) {
    return this.http.put(`${environment.appUrl}/api/manager/schedule/edit-assignment`, scheduleAssignment);
  }

  deleteAssignment(scheduleId: string) {
    return this.http.delete(`${environment.appUrl}/api/manager/schedule/delete-assignment/${scheduleId}`);
  }

  getFreeEmployees(restaurantId: string, day: Date) {
    return this.http.get(`${environment.appUrl}/api/manager/schedule/get-free-employee/${restaurantId}/${day.toDateString()}`);
  }

  getManagerFullSchedule(restaurantId: string, month: number) {
    return this.http.get(`${environment.appUrl}/api/manager/schedule/full-schedule/${restaurantId}/${month}`);
  }

  getManagerDailySchedule(restaurantId: string, day: Date) {
    return this.http.get(`${environment.appUrl}/api/manager/schedule/get-daily-schedule/${restaurantId}/${day.toDateString()}`);
  }

  logout() {
    this.userSource.next(null); // to ensure we remove the logged MANAGER {NOT USER} from HERE {NOT LOCAL STORAGE}
    this.accountService.logout();
  }

  refreshManager(email: string) {
    this.getManager(email).subscribe({
      next: (response: any) => {
        this.setManager(response);
      }
    })
  }

  setManager(manager: Manager) {
    this.userSource.next(manager);
  }
}
