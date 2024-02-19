import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { Request } from 'src/app/shared/models/requests/request';
import { RequestResponse } from 'src/app/shared/models/requests/requestResponse';
import { SharedService } from 'src/app/shared/shared.service';
import { EmployeeService } from '../../../pages-routing/employee/employee.service';
import { User } from 'src/app/shared/models/account/user';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-inbox',
  templateUrl: './inbox.component.html',
  styleUrls: ['./inbox.component.css']
})
export class InboxComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  @Output() closeDropdownEvent = new EventEmitter<void>();

  user: User | undefined;

  requests: Request[] = [];
  confirmedRequests: Request[] = [];
  unconfirmedRequests: Request[] = [];

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

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  getRequests() {
    if (this.user?.isManager) {
      const sub = this.managerService.getRequests(this.user.email).subscribe({
        next: response => {
          this.handleRequests(response);
        }
      })
      this.subscriptions.push(sub);
    }
    else if (this.user?.isManager === false) {
      const sub = this.employeeService.getRequests(this.user.email).subscribe({
        next: response => {
          this.handleRequests(response);
        }
      })
      this.subscriptions.push(sub);
    }
  }

  hasUnconfirmedRequests(response: any): boolean {
    return response.some((item: { confirmed: boolean; }) => item.confirmed === null);
  }

  splitRequestList() {
    this.confirmedRequests = this.requests.filter(item => item.confirmed !== null);
    this.unconfirmedRequests = this.requests.filter(item => item.confirmed === null);
  }

  handleRequests(response: any) {
    this.requests = response;
    this.newNotifications = this.hasUnconfirmedRequests(response);
    this.splitRequestList();
    this.sharedService.updateNotifications(this.newNotifications);
  }

  respondToRequest(confirmed: boolean, currentRequest: Request) {
    if (this.user?.email) {
      this.requestResponse.confirmed = confirmed;
      this.requestResponse.requestId = currentRequest.id;
      this.requestResponse.restaurantId = currentRequest.restaurantId;

      if (this.user.isManager) {
        const sub = this.managerService.respondToRequest(this.requestResponse).subscribe({
          next: (response: any) => {
            this.sharedService.showNotification(true, response.value.title, response.value.message);
            this.bsModalRef.hide();
            this.getRequests();
          }
        })
        this.subscriptions.push(sub);
      }
      else {
        const sub = this.employeeService.respondToRequest(this.requestResponse).subscribe({
          next: (response: any) => {
            this.sharedService.showNotification(true, response.value.title, response.value.message);
            this.bsModalRef.hide();
            this.getRequests();
            if (this.user?.email) {
              const sub1 = this.employeeService.getEmployee(this.user.email).subscribe({
                next: (response: any) => {
                  this.employeeService.setEmployee(response);
                }
              })
              this.subscriptions.push(sub1);
            }
          }
        })
        this.subscriptions.push(sub);
      }
    }
  }

  getUser() {
    const sub = this.accountService.user$.subscribe({
      next: (response: any) => {
        this.user = response;
      }
    })
    this.subscriptions.push(sub);
  }

  openSenderInfoModal(senderEmail: string) {
    if (this.user?.isManager) {
      this.sharedService.openUserInfoModal(senderEmail, 'Employee');
    }
    else {
      this.sharedService.openUserInfoModal(senderEmail, 'Manager');
    }
  }

  closeDropdown() {
    this.closeDropdownEvent.emit();
  }
}
