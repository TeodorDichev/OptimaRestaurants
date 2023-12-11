import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';

@Component({
  selector: 'app-qr-code',
  templateUrl: './qr-code.component.html',
  styleUrls: ['./qr-code.component.css']
})
export class QrCodeComponent implements OnInit {
  @Input() employeeEmail: string | undefined;
  qrCode: File | undefined;
  message: string | undefined;

  constructor(private employeeService: EmployeeService,
    public bsModalRef: BsModalRef) {}

  ngOnInit() {
    this.getQRCode();
  }

  getQRCode() {
    if (this.employeeEmail){
      this.employeeService.getQRCode(this.employeeEmail).subscribe({
        next: (response: any) => {
          this.qrCode = response;
        },
        error: error => {
          this.message = error.error;
        }
      })
    }
  }
}
