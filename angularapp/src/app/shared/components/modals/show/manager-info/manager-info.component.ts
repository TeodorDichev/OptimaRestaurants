import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Manager } from 'src/app/shared/models/manager/manager';
import { ManagerService } from 'src/app/shared/pages/manager/manager.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-manager-info',
  templateUrl: './manager-info.component.html',
  styleUrls: ['./manager-info.component.css',
    '../../../../../app.component.css']
})
export class ManagerInfoComponent implements OnInit {

  manager: Manager | undefined;

  constructor(public bsModalRef: BsModalRef,
    private managerService: ManagerService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.setManager();
  }

  editManagerProfile() {
    this.sharedService.openEditManagerModal();
    this.bsModalRef.hide();
  }


  private setManager() {
    this.managerService.manager$.subscribe({
      next: (response: any) => {
        this.manager = response;
      }
    })
  }


}
