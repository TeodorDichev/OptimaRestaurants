import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { NotificationComponent } from './components/errors/modals/notification/notification.component';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  bsModalRef?: BsModalRef;
  constructor(private modalService: BsModalService){

   }
   showNotification(isSuccess: boolean, title: string, message: string){
    const initialState: ModalOptions = {
      initialState: {
        isSuccess,
        title,
        message
      }
    };
    this.bsModalRef = this.modalService.show(NotificationComponent, initialState);
   }
}
