import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { AccountService } from '../account/account.service';
import { Employee } from 'src/app/shared/models/employee/employee';
import { environment } from 'src/environments/environment.development';
import { UpdateEmployee } from 'src/app/shared/models/employee/update-employee';

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

    deleteEmployeeAccount(email: string){
      this.logout();
      return this.http.delete(`${environment.appUrl}/api/employee/delete-employee${email}`)
    }

    updateEmployeeAccount(model: UpdateEmployee, email: string) {
      return  this.http.put(`${environment.appUrl}/api/employee/update-employee/${email}`, model);
    }

    logout() { 
      this.userSource.next(null); 
      this.accountService.logout();
    }
    
    setEmployee(employee: Employee){ 
      this.userSource.next(employee);
    }
}
