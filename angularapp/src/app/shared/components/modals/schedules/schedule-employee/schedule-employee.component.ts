import { BsModalRef } from 'ngx-bootstrap/modal';
import { Component, OnInit } from '@angular/core';
import { Employee } from 'src/app/shared/models/employee/employee';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';


@Component({
  selector: 'app-schedule-employee',
  templateUrl: './schedule-employee.component.html',
  styleUrls: ['./schedule-employee.component.css']
})
export class ScheduleEmployeeComponent implements OnInit {
  employee: Employee | undefined;

  constructor(private emplopyeeService: EmployeeService,
    private bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.getEmployee();
  }

  private getEmployee() {
    this.emplopyeeService.employee$.subscribe({
      next: (response: any) => {
        this.employee = response;
      }
    })
  }

  close() {
    this.bsModalRef.hide();
  }
}
