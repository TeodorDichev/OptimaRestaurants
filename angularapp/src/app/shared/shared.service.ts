import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { BehaviorSubject, Observable } from 'rxjs';
import { EditEmployeeComponent } from './components/modals/input/edit-employee/edit-employee.component';
import { EditManagerComponent } from './components/modals/input/edit-manager/edit-manager.component';
import { EditRestaurantModalComponent } from './components/modals/input/edit-restaurant/edit-restaurant.component';
import { NewRestaurantInputModalComponent } from './components/modals/input/new-restaurant/new-restaurant-input-modal.component';
import { NotificationComponent } from './components/modals/notification/notification.component';
import { RestaurantInfoComponent } from './components/modals/show/restaurant-info/restaurant-info.component';
import { UserInfoComponent } from './components/modals/show/user-info/user-info.component';
import { Restaurant } from './models/restaurant/restaurant';
import { QrCodeComponent } from './components/modals/qr-code/qr-code.component';
import { ScheduleManagerComponent } from './components/modals/schedules/schedule-manager/schedule-manager.component';
import { ScheduleEmployeeComponent } from './components/modals/schedules/schedule-employee/schedule-employee.component';
import { ManagerReviewComponent } from './components/modals/input/manager-review/manager-review.component';

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  private newNotificationsSubject = new BehaviorSubject<boolean>(false);
  newNotifications$: Observable<boolean> = this.newNotificationsSubject.asObservable();
  bsModalRef?: BsModalRef;

  constructor(private modalService: BsModalService) { }

  updateNotifications(value: boolean) {
    this.newNotificationsSubject.next(value);
  }

  showNotification(isSuccess: boolean, title: string, message: string) {
    const initialState: ModalOptions = {
      initialState: {
        isSuccess,
        title,
        message
      }
    };
    this.bsModalRef = this.modalService.show(NotificationComponent, initialState);
  }

  openRestaurantCreateModal() {
    this.bsModalRef = this.modalService.show(NewRestaurantInputModalComponent);
  }

  openRestaurantDetailsModal(restaurantId: string) {
    const rest: ModalOptions = {
      initialState: {
        restaurantId
      }
    }
    this.bsModalRef = this.modalService.show(RestaurantInfoComponent, Object.assign(rest, { class: 'modal-xl' }));
  }

  openRestaurantEditModal(restaurant: Restaurant | undefined) {
    const rest: ModalOptions = {
      initialState: {
        restaurant
      }
    }
    this.bsModalRef = this.modalService.show(EditRestaurantModalComponent, rest);
  }

  openEditManagerModal() {
    this.bsModalRef = this.modalService.show(EditManagerComponent);
  }

  openEditEmployeeModal() {
    this.bsModalRef = this.modalService.show(EditEmployeeComponent);
  }

  openUserInfoModal(email: string, role: string) {
    const userData: ModalOptions = {
      initialState: {
        email,
        role
      }
    }
    this.bsModalRef = this.modalService.show(UserInfoComponent, Object.assign(userData, { class: 'modal-lg' }));
  }

  openManagerReviewModal(employeeEmail: string, restaurantId: string) {
    const data: ModalOptions = {
      initialState: {
        employeeEmail,
        restaurantId
      }
    }
    this.bsModalRef = this.modalService.show(ManagerReviewComponent ,data);
  }

  openQRCodeModal() {
    this.bsModalRef = this.modalService.show(QrCodeComponent);
  }

  openScheduleManagerModal() {
    this.bsModalRef = this.modalService.show(ScheduleManagerComponent, Object.assign({ class: 'modal-fullscreen' }));
  }

  openScheduleEmployeeModal() {
    this.bsModalRef = this.modalService.show(ScheduleEmployeeComponent, Object.assign({ class: 'modal-fullscreen' }));
  }
}
