import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { EditEmployeeComponent } from './components/modals/input/edit-employee/edit-employee.component';
import { EditManagerComponent } from './components/modals/input/edit-manager/edit-manager.component';
import { EditRestaurantModalComponent } from './components/modals/input/edit-restaurant/edit-restaurant-modal.component';
import { NewRestaurantInputModalComponent } from './components/modals/input/new-restaurant/new-restaurant-input-modal.component';
import { NotificationComponent } from './components/modals/notification/notification.component';
import { RestaurantInfoComponent } from './components/modals/show/restaurant-info/restaurant-info.component';
import { Restaurant } from './models/restaurant/restaurant';

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

  openEditManagerModal() {
    this.bsModalRef = this.modalService.show(EditManagerComponent);
  }

  openEditEmployeeModal() {
    this.bsModalRef = this.modalService.show(EditEmployeeComponent);
  }

}
