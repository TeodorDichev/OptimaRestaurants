import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '../../account-pages/login/login.component';
import { RegisterManagerComponent } from '../../account-pages/register-manager/register-manager.component';
import { RegisterEmployeeComponent } from '../../account-pages/register-employee/register-employee.component';
import { ConfirmEmailComponent } from '../../account-pages/confirm-email/confirm-email.component';
import { SendEmailComponent } from '../../account-pages/send-email/send-email.component';



const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register-manager', component: RegisterManagerComponent },
  { path: 'register-employee', component: RegisterEmployeeComponent },
  { path: 'confirm-email', component: ConfirmEmailComponent },
  { path: 'send-email/:mode', component: SendEmailComponent }
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
