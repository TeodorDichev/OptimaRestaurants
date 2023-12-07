import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { Request } from 'src/app/shared/models/requests/request';
import { RequestResponse } from 'src/app/shared/models/requests/requestResponse';
import { SharedService } from 'src/app/shared/shared.service';
import { EmployeeService } from '../../pages-routing/employee/employee.service';

@Component({
  selector: 'app-inbox',
  templateUrl: './inbox.component.html',
  styleUrls: ['./inbox.component.css']
})
export class InboxComponent implements OnInit {
  @Input() email: string | undefined;
  @Input() isManager: boolean | undefined;
  requests: Request[] = [];
  requestResponse: RequestResponse = {
    confirmed: false,
    restaurantId: '',
    requestId: ''
  };

  constructor(public bsModalRef: BsModalRef,
    private managerService: ManagerService,
    private employeeService: EmployeeService,
    private sharedService: SharedService) { }

  ngOnInit() {
    this.getRequests();
  }

  getRequests() {
    if (this.email) {
      if (this.isManager) {
        this.managerService.getRequests(this.email).subscribe({
          next: (response: any) => {
            this.requests = response;
          }
        })
      }
      else {
        this.employeeService.getRequests(this.email).subscribe({
          next: (response: any) => {
            this.requests = response;
          }
        })
      }
    }
  }

  convertDate(fullDate: Date) {
    return fullDate.toString().split('T')[0]
      + ' ' +
      fullDate.toString().split('T')[1].split('.')[0];
  }

  requestToResponse(confirmed: boolean, currentRequest: Request) {
    // set values for the response dto
    if (this.email) {
      this.requestResponse.confirmed = confirmed;
      this.requestResponse.requestId = currentRequest.id;
      this.requestResponse.restaurantId = currentRequest.restaurantId;

      this.managerService.respondToRequest(this.requestResponse).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.bsModalRef.hide();
        }
      })
    }
  }
}
