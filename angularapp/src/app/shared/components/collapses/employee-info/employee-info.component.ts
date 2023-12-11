import { Component, Input } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Employee } from 'src/app/shared/models/employee/employee';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-employee-info',
  templateUrl: './employee-info.component.html',
  styleUrls: ['./employee-info.component.css']
})
export class EmployeeInfoComponent {
  @Input() employeeEmail: string | undefined;
  employee: Employee | undefined;
  employeeBirthDate: string | undefined;
  constructor(public bsModalRef: BsModalRef,
    private employeeService: EmployeeService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getEmployee();
    this.employeeBirthDate = this.employee?.birthDate.toString().split('T')[0];
  }

  editEmployeeProfile() {
    this.sharedService.openEditEmployeeModal();
    this.bsModalRef.hide();
  }

  private getEmployee() {
    if (this.employeeEmail) {
      this.employeeService.getEmployee(this.employeeEmail).subscribe({
        next: (response: any) => {
          this.employee = response;
        }
      })
    }
  }
}
