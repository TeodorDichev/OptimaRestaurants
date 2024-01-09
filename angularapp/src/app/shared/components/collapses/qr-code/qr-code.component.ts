import { Employee } from 'src/app/shared/models/employee/employee';
import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-qr-code',
  templateUrl: './qr-code.component.html',
  styleUrls: ['./qr-code.component.css']
})
export class QrCodeComponent implements OnInit {
  message: string | undefined;
  employee: Employee | undefined;

  constructor(private employeeService: EmployeeService,
    private sharedService: SharedService,
    public bsModalRef: BsModalRef) { }

  ngOnInit() {
    this.getEmployee();
  }

  getEmployee() {
    this.employeeService.employee$.subscribe({
      next: (response: any) => {
        this.employee = response;
      }
    })
  }

  downloadQRCode() {
    if (this.employee) {
      this.employeeService.getQRCode(this.employee.email).subscribe({
        next: (response: Blob) => {
          const blobUrl = window.URL.createObjectURL(response);  // Create a Blob object URL for the downloaded file       
          const a = document.createElement('a');// Create an anchor element and trigger a click to start the download
          a.href = blobUrl;
          a.download = this.employee?.firstName + '_cv.pdf';
          document.body.appendChild(a);
          a.click();
          document.body.removeChild(a); // Cleanup: Remove the anchor and revoke the Blob URL
          window.URL.revokeObjectURL(blobUrl);       
          this.sharedService.showNotification(true, 'Успешно сваляне.', 'Вашето CV беше свалено успешно!');
          this.bsModalRef.hide();
        },
        error: (error: any) => {
          console.error('Error downloading PDF:', error);
        }
      });
    }
  }
}
