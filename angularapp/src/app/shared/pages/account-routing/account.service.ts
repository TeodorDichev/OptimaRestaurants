import { Injectable } from '@angular/core';
import { RegisterEmployee } from '../../models/account/registerEmployee';
import { environment } from 'src/environments/environment.development';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RegisterManager } from '../../models/account/registerManager';
import { Login } from '../../models/account/login';
import { User } from '../../models/account/user';
import { ReplaySubject, map, of } from 'rxjs';
import { Router } from '@angular/router';
import { ConfirmEmail } from '../../models/account/confirmEmail';
import { ResetPassword } from '../../models/account/resetPassword';


@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private userSource = new ReplaySubject<User | null>(1); //
  user$ = this.userSource.asObservable();  // observable, meaning we can subscribe to it at any time

  constructor(private http: HttpClient,
    private router: Router) { }

  registerEmployee(model: RegisterEmployee) {
    return this.http.post(`${environment.appUrl}/api/account/registerEmployee`, model);
  }

  registerManager(model: RegisterManager) {
    return this.http.post(`${environment.appUrl}/api/account/registerManager`, model);
  }

  confirmEmail(model: ConfirmEmail) {
    return this.http.put(`${environment.appUrl}/api/account/confirmEmail`, model);
  }

  resendEmailConfirmationLink(email: string){
    return this.http.post(`${environment.appUrl}/api/account/resendEmailConfirmationLink/${email}`, {});
  }

  forgotUsernameOrPassword(email: string){
    return this.http.post(`${environment.appUrl}/api/account/forgotUsernameOrPassword/${email}`, {});
  }

  resetPassword(model: ResetPassword) {
    console.log(model);
    return this.http.put(`${environment.appUrl}/api/account/resetPassword`, model);
  }

  login(model: Login) {
    return this.http.post<User>(`${environment.appUrl}/api/account/login`, model).pipe(
      map((user: User) => {
        if (user) {
          this.setUser(user)
          this.router.navigateByUrl('/account/next');
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
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + jwt);

    return this.http.get<User>(`${environment.appUrl}/api/account/refresh-user-token`, {headers}).pipe(
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
