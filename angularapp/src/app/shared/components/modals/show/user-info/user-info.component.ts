import { Component, Input, OnInit } from '@angular/core';
import { Employee } from 'src/app/shared/models/employee/employee';
import { Manager } from 'src/app/shared/models/manager/manager';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent implements OnInit {
  @Input() email: string | undefined;
  @Input() role: string | undefined;
  @Input() isEmployeeFireable: boolean | undefined;
  
  employee: Employee | undefined;
  manager: Manager | undefined;

  constructor(private employeeService: EmployeeService,
    private managerService: ManagerService) { }

  ngOnInit() {
    this.getUser();
  }

  getUser() {
    if (this.email){
      if (this.role == 'Employee') {
        this.employeeService.getEmployee(this.email).subscribe({
          next: (response: any) => {
            this.employee = response;
          }
        })
      }
      else if (this.role == 'Manager') {
        this.managerService.getManager(this.email).subscribe({
          next: (response: any) => {
            this.manager = response;
          }
        })
      }
    }
  }
}
