import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ModalModule } from 'ngx-bootstrap/modal';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { ConfirmEmailComponent } from './account-pages/confirm-email/confirm-email.component';
import { IndexComponent } from './account-pages/index/index.component';
import { LoginComponent } from './account-pages/login/login.component';
import { RegisterEmployeeComponent } from './account-pages/register-employee/register-employee.component';
import { RegisterManagerComponent } from './account-pages/register-manager/register-manager.component';
import { SendEmailComponent } from './account-pages/send-email/send-email.component';
import { EmployeeInfoComponent } from './components/dropdowns/employee-info/employee-info.component';
import { InboxComponent } from './components/dropdowns/inbox/inbox.component';
import { ManagerInfoComponent } from './components/dropdowns/manager-info/manager-info.component';
import { SearchResultAccountComponent } from './components/dropdowns/search-result/account/search-result-account.component';
import { SearchResultRestaurantComponent } from './components/dropdowns/search-result/restaurant/search-result-restaurant.component';
import { ValidationMessagesComponent } from './components/errors/validation-messages/validation-messages.component';
import { RateEmployeeRestaurantComponent } from './components/misc/rate-employee-restaurant/rate-employee-restaurant.component';
import { RestaurantMapComponent } from './components/misc/restaurant-map/restaurant-map.component';
import { StarRatingComponent } from './components/misc/star-rating/star-rating.component';
import { EditEmployeeComponent } from './components/modals/input/edit-employee/edit-employee.component';
import { EditManagerComponent } from './components/modals/input/edit-manager/edit-manager.component';
import { EditRestaurantModalComponent } from './components/modals/input/edit-restaurant/edit-restaurant.component';
import { NewRestaurantInputModalComponent } from './components/modals/input/new-restaurant/new-restaurant-input-modal.component';
import { NotificationComponent } from './components/modals/notification/notification.component';
import { QrCodeComponent } from './components/modals/qr-code/qr-code.component';
import { ScheduleEmployeeComponent } from './components/modals/schedules/schedule-employee/schedule-employee.component';
import { ScheduleManagerComponent } from './components/modals/schedules/schedule-manager/schedule-manager.component';
import { RestaurantInfoComponent } from './components/modals/show/restaurant-info/restaurant-info.component';
import { UserInfoComponent } from './components/modals/show/user-info/user-info.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { EmployeeLoggedViewComponent } from './logged-pages/employee-logged-view/employee-logged-view.component';
import { EmployeesLookingForJobComponent } from './logged-pages/employees-looking-for-job/employees-looking-for-job.component';
import { ManagerLoggedViewComponent } from './logged-pages/manager-logged-view/manager-logged-view.component';
import { AboutComponent } from './user-pages/about/about.component';
import { BrowseAllRestaurantsComponent } from './user-pages/browse-all-restaurants/browse-all-restaurants.component';
import { ReviewsComponent } from './user-pages/reviews/review.component';
import { ManagerReviewComponent } from './components/modals/input/manager-review/manager-review.component';

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
    NewRestaurantInputModalComponent,
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
    AboutComponent,
    RateEmployeeRestaurantComponent,
    EditRestaurantModalComponent,
    ScheduleEmployeeComponent,
    ScheduleManagerComponent,
    RestaurantMapComponent,
    ManagerReviewComponent 
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    HttpClientModule,
    ModalModule.forRoot(),
    FormsModule,
    TooltipModule,
    PopoverModule
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
