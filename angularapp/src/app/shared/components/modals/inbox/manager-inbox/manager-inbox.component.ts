import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Manager } from 'src/app/shared/models/manager/manager';
import { ManagerService } from 'src/app/shared/pages/page-routing/manager/manager.service';


@Component({
  selector: 'app-manager-inbox',
  templateUrl: './manager-inbox.component.html',
  styleUrls: ['./manager-inbox.component.css']
})
export class ManagerInboxComponent {
  manager: Manager | undefined;

  constructor(public bsModalRef: BsModalRef,
    private managerService: ManagerService) { }

  ngOnInit(): void {
    this.setManager();
  }


  private setManager() {
    this.managerService.manager$.subscribe({
      next: (response: any) => {
        this.manager = response;
      }
    })
  }

}
