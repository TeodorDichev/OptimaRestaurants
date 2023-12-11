import { Employee } from 'src/app/shared/models/employee/employee';
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
  message: string | undefined;
  employee: Employee | undefined;

  constructor(private employeeService: EmployeeService,
    public bsModalRef: BsModalRef) {}

  ngOnInit() {
    this.getEmployee();
  }

  getEmployee() {
    if (this.employeeEmail){
      this.employeeService.getEmployee(this.employeeEmail).subscribe({
        next: (response: any) => {
          this.employee = response;
        }
      })
    } 
  }

  downloadQRCode() {
    if (this.employeeEmail){
      this.employeeService.getQRCode(this.employeeEmail).subscribe({
        next: (response: Blob) => {
          const blobUrl = window.URL.createObjectURL(response);  // Create a Blob object URL for the downloaded file       
          const a = document.createElement('a');// Create an anchor element and trigger a click to start the download
          a.href = blobUrl;
          a.download = this.employee?.firstName + '_cv.pdf';
          document.body.appendChild(a);
          a.click();
          document.body.removeChild(a); // Cleanup: Remove the anchor and revoke the Blob URL
          window.URL.revokeObjectURL(blobUrl);
        },
        error: (error: any) => {
          console.error('Error downloading PDF:', error);
        }
      });
    }
  }
}
