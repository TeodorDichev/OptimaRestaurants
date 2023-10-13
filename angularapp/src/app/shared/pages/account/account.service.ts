import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../../models/register';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http: HttpClient) 
  { }
  register(model: Register){
    return this.http.post(`${environment.appUrl}/api/account/register`, model);
  }
}
