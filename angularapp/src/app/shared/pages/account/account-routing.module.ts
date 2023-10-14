import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '../login/login.component';
import { SignupManagerComponent } from '../signupmanager/signupmanager.component';
import { RegisterEmployeeComponent } from '../registerEmployee/registerEmployee.component';


const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'signupmanager', component: SignupManagerComponent},
  {path: 'registerEmployee', component: RegisterEmployeeComponent}
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class AccountRoutingModule { }
