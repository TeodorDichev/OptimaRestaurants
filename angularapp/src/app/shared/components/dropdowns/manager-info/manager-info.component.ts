import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { Manager } from 'src/app/shared/models/manager/manager';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-manager-info',
  templateUrl: './manager-info.component.html',
  styleUrls: ['./manager-info.component.css']
})
export class ManagerInfoComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  manager: Manager | undefined;

  constructor(public bsModalRef: BsModalRef,
    private managerService: ManagerService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getManager();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  editManagerProfile() {
    this.sharedService.openEditManagerModal();
    this.bsModalRef.hide();
  }

  private getManager() {
    const sub = this.managerService.manager$.subscribe({
      next: (response: any) => {
        this.manager = response;
      }
    })
    this.subscriptions.push(sub);
  }

  missingIcon(manager: Manager) { 
    manager.profilePicturePath = 'assets/images/logo-bw-with-bg.png';
  }
}
