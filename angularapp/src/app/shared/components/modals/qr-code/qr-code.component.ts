import { Component, OnDestroy, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { Employee } from 'src/app/shared/models/employee/employee';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-qr-code',
  templateUrl: './qr-code.component.html',
  styleUrls: ['./qr-code.component.css']
})
export class QrCodeComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  message: string | undefined;
  employee: Employee | undefined;

  constructor(private employeeService: EmployeeService,
    private sharedService: SharedService,
    public bsModalRef: BsModalRef) { }

  ngOnInit() {
    this.getEmployee();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  getEmployee() {
    const sub = this.employeeService.employee$.subscribe({
      next: (response: any) => {
        this.employee = response;
      }
    });
    this.subscriptions.push(sub);
  }

  downloadQRCode() {
    if (this.employee) {
      const sub = this.employeeService.getQRCode(this.employee.email).subscribe({
        next: (response: Blob) => {
          const blobUrl = window.URL.createObjectURL(response);  // Create a Blob object URL for the downloaded file       
          const a = document.createElement('a');// Create an anchor element and trigger a click to start the download
          a.href = blobUrl;
          a.download = this.employee?.firstName + '_qr.png';
          document.body.appendChild(a);
          a.click();
          document.body.removeChild(a); // Cleanup: Remove the anchor and revoke the Blob URL
          window.URL.revokeObjectURL(blobUrl);
          this.sharedService.showNotification(true, 'Успешно сваляне.', 'Вашият QR код беше свален успешно!');
          this.bsModalRef.hide();
        },
        error: (error: any) => {
          console.error('Error downloading PDF:', error);
        }
      });
      this.subscriptions.push(sub);
    }
  }

  regenerateQRCode() {
    if (this.employee) {
      const sub = this.employeeService.regenerateQRCode(this.employee.email).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.bsModalRef.hide();
        }, error: error => {
          this.sharedService.showNotification(false, 'Неуспешно регенериране', error.error);
          this.bsModalRef.hide();
        }
      });
      this.subscriptions.push(sub);
    }
  }
}
