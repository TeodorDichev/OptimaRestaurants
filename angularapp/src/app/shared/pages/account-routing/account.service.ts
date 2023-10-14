import { Injectable } from '@angular/core';
import { RegisterEmployee } from '../../models/registerEmployee';
import { environment } from 'src/environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { RegisterManager } from '../../models/registerManager';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http: HttpClient) 
  { }
  registerEmployee(model: RegisterEmployee){
    return this.http.post(`${environment.appUrl}/api/account/registerEmployee`, model);
  }
  registerManager(model: RegisterManager){
    return this.http.post(`${environment.appUrl}/api/account/registerManager`, model);
  }
}
