import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { Request } from 'src/app/shared/models/requests/request';

@Component({
  selector: 'app-manager-inbox',
  templateUrl: './manager-inbox.component.html',
  styleUrls: ['./manager-inbox.component.css']
})
export class ManagerInboxComponent implements OnInit {
  @Input() email: string | undefined;
  requests: Request[] = [];

  constructor(public bsModalRef: BsModalRef,
    private managerService: ManagerService) { }

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
  
    requestResponse(confirmed: boolean){
      
    }

}
