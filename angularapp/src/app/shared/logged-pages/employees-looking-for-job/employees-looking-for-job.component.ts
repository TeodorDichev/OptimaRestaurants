import { Component, OnDestroy, OnInit } from '@angular/core';
import { Employee } from '../../models/employee/employee';
import { ManagerService } from '../../pages-routing/manager/manager.service';
import { SharedService } from '../../shared.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-employees-looking-for-job',
  templateUrl: './employees-looking-for-job.component.html',
  styleUrls: ['./employees-looking-for-job.component.css']
})
export class EmployeesLookingForJobComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  employeesLookingForJob: Employee[] = [];

  constructor(private managerService: ManagerService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getEmployeesLookingForJob();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  getEmployeesLookingForJob() {
    const sub = this.managerService.getEmployeesLookingForJob().subscribe({
      next: (response: any) => {
        this.employeesLookingForJob = response;
      }
    });
    this.subscriptions.push(sub);
  }

  getEmployeeInfo(employeeEmail: string) {
    this.sharedService.openUserInfoModal(employeeEmail, 'Employee');
  }
}
