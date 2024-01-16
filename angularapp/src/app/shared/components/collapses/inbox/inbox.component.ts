import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { Request } from 'src/app/shared/models/requests/request';
import { RequestResponse } from 'src/app/shared/models/requests/requestResponse';
import { SharedService } from 'src/app/shared/shared.service';
import { EmployeeService } from '../../../pages-routing/employee/employee.service';
import { User } from 'src/app/shared/models/account/user';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';

@Component({
  selector: 'app-inbox',
  templateUrl: './inbox.component.html',
  styleUrls: ['./inbox.component.css']
})
export class InboxComponent implements OnInit {
  user: User | undefined;
  requests: Request[] = [];
  requestResponse: RequestResponse = {
    confirmed: false,
    restaurantId: '',
    requestId: ''
  };
  newNotifications: boolean = false;

  constructor(public bsModalRef: BsModalRef,
    private managerService: ManagerService,
    private employeeService: EmployeeService,
    private accountService: AccountService,
    private sharedService: SharedService) { }

  ngOnInit() {
    this.getUser();
    this.getRequests();
  }

  getRequests() {
    if (this.user?.isManager) {
      this.managerService.getRequests(this.user.email).subscribe({
        next: (response: any) => {
          this.requests = response;

          const hasUnconfirmedRequest = response.some((item: { confirmed: boolean; }) => item.confirmed === null);
          if (hasUnconfirmedRequest) {
            this.newNotifications = true;
          } else {
            this.newNotifications = false;
          }

          this.sharedService.updateNotifications(this.newNotifications);
        }
      })
    }
    else if (this.user?.isManager == false) {
      this.employeeService.getRequests(this.user.email).subscribe({
        next: (response: any) => {
          this.requests = response;

          const hasUnconfirmedRequest = response.some(
            (item: { confirmed: boolean; }) =>
              item.confirmed === null);

          if (hasUnconfirmedRequest) {
            this.newNotifications = true;
          } else {
            this.newNotifications = false;
          }

          this.sharedService.updateNotifications(this.newNotifications);
        }
      })
    }
  }

  respondToRequest(confirmed: boolean, currentRequest: Request) {
    if (this.user?.email) {
      this.requestResponse.confirmed = confirmed;
      this.requestResponse.requestId = currentRequest.id;
      this.requestResponse.restaurantId = currentRequest.restaurantId;

      if (this.user.isManager) {
        this.managerService.respondToRequest(this.requestResponse).subscribe({
          next: (response: any) => {
            this.sharedService.showNotification(true, response.value.title, response.value.message);
            this.bsModalRef.hide();
            this.getRequests();
          }
        })
      }
      else {
        this.employeeService.respondToRequest(this.requestResponse).subscribe({
          next: (response: any) => {
            this.sharedService.showNotification(true, response.value.title, response.value.message);
            this.bsModalRef.hide();
            this.getRequests();
            if (this.user?.email){
              this.employeeService.getEmployee(this.user?.email).subscribe({
                next: (response: any) => {
                  this.employeeService.setEmployee(response);
                }
              })
            }
          }
        })
      }
    }

  }

  getUser() {
    this.accountService.user$.subscribe({
      next: (response: any) => {
        this.user = response;
      }
    })
  }
}
