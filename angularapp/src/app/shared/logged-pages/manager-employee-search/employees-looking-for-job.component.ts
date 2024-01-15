import { Component, OnInit } from '@angular/core';
import { Employee } from '../../models/employee/employee';
import { ManagerService } from '../../pages-routing/manager/manager.service';
import { SharedService } from '../../shared.service';

@Component({
  selector: 'app-employees-looking-for-job',
  templateUrl: './employees-looking-for-job.component.html',
  styleUrls: ['./employees-looking-for-job.component.css']
})
export class EmployeesLookingForJobComponent implements OnInit {
  employeesLookingForJob: Employee[] = [];

  constructor(private managerService: ManagerService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getEmployeesLookingForJob();
  }

  getEmployeesLookingForJob() {
    this.managerService.getEmployeesLookingForJob().subscribe({
      next: (response: any) => {
        this.employeesLookingForJob = response;
      }
    });
  }
}
