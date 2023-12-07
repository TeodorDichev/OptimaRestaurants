import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { NotificationComponent } from './components/modals/notification/notification.component';
import { NewRestaurantInputModalComponent } from './components/modals/input/new-restaurant/new-restaurant-input-modal.component';
import { EditRestaurantModalComponent } from './components/modals/input/edit-restaurant/edit-restaurant-modal.component';
import { EditManagerComponent } from './components/modals/input/edit-manager/edit-manager.component';
import { EditEmployeeComponent } from './components/modals/input/edit-employee/edit-employee.component';
import { ManagerInfoComponent } from './components/modals/show/manager-info/manager-info.component';
import { EmployeeInfoComponent } from './components/modals/show/employee-info/employee-info.component';
import { Restaurant } from './models/restaurant/restaurant';
import { QrCodeComponent } from './components/modals/show/qr-code/qr-code.component';
import { CvModalComponent } from './components/modals/show/cv-modal/cv-modal.component';
import { RestaurantInfoComponent } from './components/modals/show/restaurant-info/restaurant-info.component';
import { User } from './models/account/user';
import { InboxComponent } from './components/collapses/inbox.component';

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  bsModalRef?: BsModalRef;
  constructor(private modalService: BsModalService) {

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

  openRestaurantDetailsModal(restaurant: Restaurant) {
    const rest: ModalOptions = {
      initialState: {
        restaurant
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

  openManagerInfoModal() {
    this.bsModalRef = this.modalService.show(ManagerInfoComponent);
  }

  openEmployeeInfoModal() {
    this.bsModalRef = this.modalService.show(EmployeeInfoComponent);
  }

  openEditManagerModal() {
    this.bsModalRef = this.modalService.show(EditManagerComponent);
  }

  openEditEmployeeModal() {
    this.bsModalRef = this.modalService.show(EditEmployeeComponent);
  }

  openQRCodeModal() {
    this.bsModalRef = this.modalService.show(QrCodeComponent);
  }

  openCVModal() {
    this.bsModalRef = this.modalService.show(CvModalComponent);
  }
}
