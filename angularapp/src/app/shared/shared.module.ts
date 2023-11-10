import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ValidationMessagesComponent } from './components/errors/validation-messages/validation-messages.component';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NotificationComponent } from './components/modals/notification/notification.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ConfirmEmailComponent } from './pages/confirm-email/confirm-email.component';
import { SendEmailComponent } from './pages/send-email/send-email.component';
import { ResetPasswordComponent } from './pages/reset-password/reset-password.component';
import { NewRestaurantInputModalComponent } from './components/modals/input/new-restaurant/new-restaurant-input-modal.component';
import { EditRestaurantModalComponent } from './components/modals/input/edit-restaurant/edit-restaurant-modal.component';
import { EditManagerComponent } from './components/modals/input/edit-manager/edit-manager.component';
import { EditEmployeeComponent } from './components/modals/input/edit-employee/edit-employee.component';
import { ManagerInfoComponent } from './components/dropdowns/manager-info/manager-info.component';
import { EmployeeInfoComponent } from './components/dropdowns/employee-info/employee-info.component';


@NgModule({
  declarations: [
    ValidationMessagesComponent,
    NotificationComponent,
    ConfirmEmailComponent,
    SendEmailComponent,
    ResetPasswordComponent,
    NewRestaurantInputModalComponent,
    EditRestaurantModalComponent,
    EditManagerComponent,
    EditEmployeeComponent,
    ManagerInfoComponent,
    EmployeeInfoComponent
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
