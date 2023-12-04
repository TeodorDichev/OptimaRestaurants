import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Request } from 'src/app/shared/models/requests/request';
import { RequestResponse } from 'src/app/shared/models/requests/requestResponse';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-employee-inbox',
  templateUrl: './employee-inbox.component.html',
  styleUrls: ['./employee-inbox.component.css']
})
export class EmployeeInboxComponent implements OnInit {
  @Input() email: string | undefined; // employee email
  requests: Request[] = [];
  requestResponse: RequestResponse = {
    confirmed: false,
    currentUserEmail: '',
    restaurantId: '',
    requestId: ''
  };

  constructor(public bsModalRef: BsModalRef,
    private employeeService: EmployeeService,
    private sharedService: SharedService) { }

  ngOnInit() {
    this.getRequests();
  }

  getRequests() {
    if (this.email) {
      this.employeeService.getRequests(this.email).subscribe({
        next: (response: any) => {
          this.requests = response;
        }
      })
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
      this.requestResponse.currentUserEmail = this.email;
      this.requestResponse.requestId = currentRequest.id;
      this.requestResponse.restaurantId = currentRequest.restaurantId;

      console.log(this.requestResponse);
      this.employeeService.respondToRequest(this.requestResponse).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.bsModalRef.hide();
        }
      })
    }
  }
}
