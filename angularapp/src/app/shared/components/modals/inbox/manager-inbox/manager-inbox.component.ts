import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { Request } from 'src/app/shared/models/requests/request';
import { RequestResponse } from 'src/app/shared/models/requests/requestResponse';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-manager-inbox',
  templateUrl: './manager-inbox.component.html',
  styleUrls: ['./manager-inbox.component.css']
})
export class ManagerInboxComponent implements OnInit {
  @Input() email: string | undefined; // manager email
  requests: Request[] = [];
  requestResponse: RequestResponse = {
    confirmed: false,
    restaurantId: '',
    requestId: ''
  };

  constructor(public bsModalRef: BsModalRef,
    private managerService: ManagerService,
    private sharedService: SharedService) { }

  ngOnInit() {
    this.getRequests();
  }

  getRequests() {
    if (this.email) {
      this.managerService.getRequests(this.email).subscribe({
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
      this.requestResponse.requestId = currentRequest.id;
      this.requestResponse.restaurantId = currentRequest.restaurantId;

      console.log(this.requestResponse);
      this.managerService.respondToRequest(this.requestResponse).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.bsModalRef.hide();

        }
      })
    }
  }
}
