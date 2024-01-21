import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { take } from 'rxjs';
import { Manager } from 'src/app/shared/models/manager/manager';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-manager-info',
  templateUrl: './manager-info.component.html',
  styleUrls: ['./manager-info.component.css']
})
export class ManagerInfoComponent implements OnInit {
  @Input() managerEmail: string | undefined;
  manager: Manager | undefined;

  constructor(public bsModalRef: BsModalRef,
    private managerService: ManagerService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getManager();
  }

  editManagerProfile() {
    this.sharedService.openEditManagerModal();
    this.bsModalRef.hide();
  }

  private getManager() {
    if(this.managerEmail) {
      this.managerService.manager$.subscribe({  
        next: (response: any) => {
          this.manager = response;
        }
      })
    }
  }
}
