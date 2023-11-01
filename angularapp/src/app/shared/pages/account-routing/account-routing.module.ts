import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '../login/login.component';
import { RegisterManagerComponent } from '../register-manager/register-manager.component';
import { RegisterEmployeeComponent } from '../register-employee/register-employee.component';
import { ConfirmEmailComponent } from '../confirm-email/confirm-email.component';
import { SendEmailComponent } from '../send-email/send-email.component';
import { ResetPasswordComponent } from '../reset-password/reset-password.component';
import { EmployeeLoggedViewComponent } from '../employee-logged-view/employee-logged-view/employee-logged-view.component';
import { ManagerLoggedViewComponent } from '../manager-logged-view/manager-logged-view/manager-logged-view.component';


const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'register-manager', component: RegisterManagerComponent},
  {path: 'register-employee', component: RegisterEmployeeComponent},
  {path: 'confirm-email', component: ConfirmEmailComponent},
  {path: 'send-email/:mode', component: SendEmailComponent},
  {path: 'reset-password', component: ResetPasswordComponent},
  {path: 'employee-logged-view', component: EmployeeLoggedViewComponent},
  {path: 'manager-logged-view', component: ManagerLoggedViewComponent}
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
