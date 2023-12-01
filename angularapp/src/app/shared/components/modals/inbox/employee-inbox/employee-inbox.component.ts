import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';

@Component({
  selector: 'app-employee-inbox',
  templateUrl: './employee-inbox.component.html',
  styleUrls: ['./employee-inbox.component.css']
})
export class EmployeeInboxComponent implements OnInit {
  @Input() email: string | undefined;
  requests: Request[] = [];

  constructor(private employeeService: EmployeeService,
    public bsModalRef: BsModalRef) { }

  ngOnInit() {
    this.getRequests();
  }

  getRequests() {
    if (this.email) {
      this.employeeService.getRequests(this.email).subscribe({
        next: (response: any) => {
          this.requests = response;
        }
      })
    }
  }

  requestResponse(confirmed: boolean){
    
  }
}
