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

    const formData: FormData = new FormData(); // for possible file sending, otherwise I send a link to the image (only way i found)

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

  logout() {
    this.userSource.next(null);
    this.accountService.logout();
  }

  refreshEmployee(email: string) {
    this.getEmployee(email).subscribe({
      next: (response: any) => {
        this.setEmployee(response);
      }
    })
  }

  setEmployee(employee: Employee) {
    this.userSource.next(employee);
  }
}
