import { Injectable } from '@angular/core';
import { RegisterEmployee } from '../../models/register.employee';
import { environment } from 'src/environments/environment.development';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http: HttpClient) 
  { }
  registerEmployee(model: RegisterEmployee){
    return this.http.post(`${environment.appUrl}/api/account/registerEmployee`, model);
  }
}
