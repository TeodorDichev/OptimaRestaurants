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

  currentPage: number = 1;
  totalPages: number = 1;
  totalEmployeeCount: number = 0;

  constructor(private managerService: ManagerService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getEmployeesOnPage();
    this.getEmployeeCount();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  getEmployeeInfo(employeeEmail: string) {
    this.sharedService.openUserInfoModal(employeeEmail, 'Employee');
  }

  previousPage() {
    if (this.currentPage != 1) {
      this.currentPage -= 1;
      this.getEmployeesOnPage();
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage += 1;
      this.getEmployeesOnPage();
    }
  }

  missingIcon(employee: Employee) {
    //employee.profilePicturePath = 'assets/images/logo-bw-with-bg.png';
  }

  getEmployeeCount() {
    const sub = this.managerService.getCountOfEmployeesLookingForJob().subscribe({
      next: (response: any) => {
        this.totalPages = Math.ceil(response / 20);
        this.totalEmployeeCount = response;
      }
    });
    this.subscriptions.push(sub);
  }

  getEmployeesOnPage() {
    const sub = this.managerService.getEmployeesLookingForJob(this.currentPage).subscribe({
      next: (response: any) => {
        this.employeesLookingForJob = response;
      }
    });
    this.subscriptions.push(sub);
  }

  setCurrentPage(pageIndex: number) {
    this.currentPage = pageIndex;
    this.getEmployeesOnPage();
  }
}
