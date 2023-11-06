import { Injectable } from '@angular/core';
import { RegisterEmployee } from '../../../models/account/register-employee';
import { environment } from 'src/environments/environment.development';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RegisterManager } from '../../../models/account/register-manager';
import { Login } from '../../../models/account/login';
import { User } from '../../../models/account/user';
import { ReplaySubject, map, of, take, timer } from 'rxjs';
import { Router } from '@angular/router';
import { ConfirmEmail } from '../../../models/account/confirm-email';
import { ResetPassword } from '../../../models/account/reset-password';


@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private userSource = new ReplaySubject<User | null>(1); 
  user$ = this.userSource.asObservable();  // observable, meaning we can subscribe to it at any time

  constructor(private http: HttpClient,
    private router: Router) { }

  registerEmployee(model: RegisterEmployee) {
    return this.http.post<User>(`${environment.appUrl}/api/account/register-employee`, model).pipe(
      map((user: User) => {
        if (user) {
          this.setUser(user);
          return user;
        }
        return null;
      })
    );
  }

  registerManager(model: RegisterManager) {
    return this.http.post<User>(`${environment.appUrl}/api/account/register-manager`, model).pipe(
      map((user: User) => {
        if (user) {
          this.setUser(user);
          return user;
        }
        return null;
      })
    );
  }

  confirmEmail(model: ConfirmEmail) {
    return this.http.put<User>(`${environment.appUrl}/api/account/confirm-email`, model).pipe(
      map((user: User) => {
        if (user) {
          this.setUser(user);
          console.log(this.user$);
          return user;
        }
        return null;
      })
    );;
  }

  resendEmailConfirmationLink(email: string){
    return this.http.post(`${environment.appUrl}/api/account/resend-email-confirmation-link/${email}`, {});
  }

  forgotUsernameOrPassword(email: string){
    return this.http.post(`${environment.appUrl}/api/account/forgot-username-or-password/${email}`, {});
  }

  resetPassword(model: ResetPassword) {
    console.log(model);
    return this.http.put(`${environment.appUrl}/api/account/reset-password`, model);
  }

  login(model: Login) {
    return this.http.post<User>(`${environment.appUrl}/api/account/login`, model).pipe(
      map((user: User) => {
        if (user) {
          this.setUser(user);
          if (user.isManager){
          this.router.navigateByUrl('/manager');
          }
          else {
            this.router.navigateByUrl('/employee');
          }
          return user;
        }
        return null;
      })
    );
  }

  logout(){
    localStorage.removeItem(environment.userKey);
    this.userSource.next(null);
    this.router.navigateByUrl('/');
  }
  getJWT() {
    const key = localStorage.getItem(environment.userKey);
    if (key) {
      const user: User = JSON.parse(key);
      return user.jwt;
    } else {
      return null;
    }
  }

  refreshUser(jwt: string | null){
    if (jwt === null){
      this.userSource.next(null);
      return of(undefined);
    } 
    console.log(jwt);
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + jwt);
    return this.http.get<User>(`${environment.appUrl}/api/account/refresh-user-token`, {headers}).pipe(
      take(1),
      map((user: User) => {
        if (user) {
          this.setUser(user);
        }
      })
    );
  }
  private setUser(user: User) {
    localStorage.setItem(environment.userKey, JSON.stringify(user));
    this.userSource.next(user); // we store the user in the local storage in browser and in the angular app - to tell whether the user is logged in and keep him logged after refreshing page
  }
}
