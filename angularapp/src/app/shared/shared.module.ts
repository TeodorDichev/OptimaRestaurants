import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ValidationMessagesComponent } from './components/errors/validation-messages/validation-messages.component';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NotificationComponent } from './components/errors/modals/notification/notification.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ConfirmEmailComponent } from './pages/confirm-email/confirm-email.component';
import { SendEmailComponent } from './pages/send-email/send-email.component';
import { ResetPasswordComponent } from './pages/reset-password/reset-password.component';
import { ManagerLoggedViewComponent } from './pages/manager-logged-view/manager-logged-view/manager-logged-view.component';
import { EmployeeLoggedViewComponent } from './pages/employee-logged-view/employee-logged-view/employee-logged-view.component';

@NgModule({
  declarations: [
    ValidationMessagesComponent,
    ValidationMessagesComponent,
    NotificationComponent,
    ConfirmEmailComponent,
    SendEmailComponent,
    ResetPasswordComponent,
    ManagerLoggedViewComponent,
    EmployeeLoggedViewComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    HttpClientModule,
    ModalModule.forRoot()
  ],
  exports: [
    RouterModule,
    ReactiveFormsModule,
    HttpClientModule,
    ValidationMessagesComponent
  ]
})
export class SharedModule { }
