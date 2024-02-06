import { ScheduleAssignment } from './../../models/employee/schedule-assignent';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { AccountService } from '../account/account.service';
import { Employee } from 'src/app/shared/models/employee/employee';
import { environment } from 'src/environments/environment.development';
import { UpdateEmployee } from 'src/app/shared/models/employee/update-employee';
import { RequestResponse } from '../../models/requests/requestResponse';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private userSource = new ReplaySubject<Employee | null>(1);
  employee$ = this.userSource.asObservable();

  constructor(private http: HttpClient,
    private accountService: AccountService) { }

  getEmployee(email: string) {
    return this.http.get(`${environment.appUrl}/api/employee/get-employee/${email}`);
  }

  deleteEmployeeAccount(email: string) {
    this.logout();
    return this.http.delete(`${environment.appUrl}/api/employee/delete-employee/${email}`);
  }

  updateEmployeeAccount(model: UpdateEmployee, email: string) {

    const formData: FormData = new FormData(); 

    formData.append('newFirstName', model.newFirstName);
    formData.append('newLastName', model.newLastName);
    formData.append('newPhoneNumber', model.newPhoneNumber);
    formData.append('profilePictureFile', model.profilePictureFile);
    formData.append('newBirthDate', model.newBirthDate.toString());
    formData.append('newCity', model.newCity);
    formData.append('isLookingForJob', model.isLookingForJob.toString());

    return this.http.put(`${environment.appUrl}/api/employee/update-employee/${email}`, formData);
  }

  getRequests(email: string) {
    return this.http.get(`${environment.appUrl}/api/employee/get-all-requests/${email}`);
  }

  respondToRequest(requestResponse: RequestResponse) {
    return this.http.post(`${environment.appUrl}/api/employee/respond-to-request`, requestResponse);
  }

  getQRCode(email: string): Observable<Blob> {
    return this.http.get(`${environment.appUrl}/api/employee/download-qrcode/${email}`, { responseType: 'blob' });
  }

  getPDFFile(email: string): Observable<Blob> {
    return this.http.get(`${environment.appUrl}/api/employee/download-cv/${email}`, { responseType: 'blob' });
  }

  getEmployeeRestaurantSchedule(email: string, restaurantId: string, month: number) {
    return this.http.get(`${environment.appUrl}/api/employee/get-restaurant-schedule/${email}/${restaurantId}/${month}`);
  }

  getEmployeeFullSchedule(email: string, month: number) {
    return this.http.get(`${environment.appUrl}/api/employee/get-full-schedule/${email}/${month}`);
  }

  getDailySchedule(email: string, day: Date) {
    return this.http.get(`${environment.appUrl}/api/employee/get-day-schedule/${email}/${day}`);
  }

  addAssignment(scheduleAssignment: ScheduleAssignment) {
    return this.http.post(`${environment.appUrl}/api/employee/schedule/add-assignment`, scheduleAssignment);
  }

  editAssignment(scheduleAssignment: ScheduleAssignment) {
    return this.http.post(`${environment.appUrl}/api/employee/schedule/edit-assignment`, scheduleAssignment);
  }

  deleteAssignment(scheduleId: string) {
    return this.http.delete(`${environment.appUrl}/api/employee/schedule/delete-assignment/${scheduleId}`);
  }

  logout() {
    this.userSource.next(null);
    this.accountService.logout();
  }

  refreshEmployee(email: string) {
    this.getEmployee(email).subscribe({
      next: (response: any) => {
        this.setEmployee(response);
      },
      error: error => {
        console.error(error);
      }
    })
  }

  setEmployee(employee: Employee) {
    this.userSource.next(employee);
  }
}
