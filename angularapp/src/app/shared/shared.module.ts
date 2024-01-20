import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ConfirmEmailComponent } from './account-pages/confirm-email/confirm-email.component';
import { IndexComponent } from './account-pages/index/index.component';
import { LoginComponent } from './account-pages/login/login.component';
import { RegisterEmployeeComponent } from './account-pages/register-employee/register-employee.component';
import { RegisterManagerComponent } from './account-pages/register-manager/register-manager.component';
import { ResetPasswordComponent } from './account-pages/reset-password/reset-password.component';
import { SendEmailComponent } from './account-pages/send-email/send-email.component';
import { InboxComponent } from './components/collapses/inbox/inbox.component';
import { ManagerInfoComponent } from './components/collapses/manager-info/manager-info.component';
import { QrCodeComponent } from './components/collapses/qr-code/qr-code.component';
import { ValidationMessagesComponent } from './components/errors/validation-messages/validation-messages.component';
import { StarRatingComponent } from './components/misc/star-rating/star-rating.component';
import { EditEmployeeComponent } from './components/modals/input/edit-employee/edit-employee.component';
import { EditManagerComponent } from './components/modals/input/edit-manager/edit-manager.component';
import { EditRestaurantModalComponent } from './components/modals/input/edit-restaurant/edit-restaurant-modal.component';
import { NewRestaurantInputModalComponent } from './components/modals/input/new-restaurant/new-restaurant-input-modal.component';
import { NotificationComponent } from './components/modals/notification/notification.component';
import { RestaurantInfoComponent } from './components/modals/show/restaurant-info/restaurant-info.component';
import { EmployeeLoggedViewComponent } from './logged-pages/employee-logged-view/employee-logged-view.component';
import { EmployeesLookingForJobComponent } from './logged-pages/manager-employee-search/employees-looking-for-job.component';
import { ManagerLoggedViewComponent } from './logged-pages/manager-logged-view/manager-logged-view.component';
import { BrowseAllRestaurantsComponent } from './restaurants/browse-all-restaurants/browse-all-restaurants.component';
import { EmployeeInfoComponent } from './components/collapses/employee-info/employee-info.component';
import { SearchResultAccountComponent } from './components/collapses/search-result/account/search-result-account.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { UserInfoComponent } from './components/modals/show/user-info/user-info.component';
import { SearchResultRestaurantComponent } from './components/collapses/search-result/restaurant/search-result-restaurant.component';
import { ReviewsComponent } from './restaurants/reviews/reviews.component';
import { AboutComponent } from './restaurants/about/about.component';

@NgModule({
  declarations: [
    ValidationMessagesComponent,
    LoginComponent,
    RegisterManagerComponent,
    IndexComponent,
    RegisterEmployeeComponent,
    ManagerLoggedViewComponent,
    EmployeeLoggedViewComponent,
    EmployeeInfoComponent,
    StarRatingComponent,
    BrowseAllRestaurantsComponent,
    RestaurantInfoComponent,
    EmployeesLookingForJobComponent,
    NotificationComponent,
    ConfirmEmailComponent,
    SendEmailComponent,
    ResetPasswordComponent,
    NewRestaurantInputModalComponent,
    EditRestaurantModalComponent,
    EditManagerComponent,
    EditEmployeeComponent,
    ManagerInfoComponent,
    QrCodeComponent,
    InboxComponent,   
    SearchResultAccountComponent,
    NavbarComponent,
    UserInfoComponent,
    SearchResultRestaurantComponent,
    ReviewsComponent,
    AboutComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    HttpClientModule,
    ModalModule.forRoot(),
    TooltipModule
  ],
  exports: [
    RouterModule,
    ReactiveFormsModule,
    HttpClientModule,
    ValidationMessagesComponent,
    QrCodeComponent,
    EmployeeInfoComponent,
    InboxComponent,
    SearchResultAccountComponent, 
    ManagerInfoComponent
  ]
})
export class SharedModule { }
