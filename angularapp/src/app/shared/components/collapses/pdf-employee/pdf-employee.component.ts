import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-pdf-employee',
  templateUrl: './pdf-employee.component.html',
  styleUrls: ['./pdf-employee.component.css']
})
export class PdfEmployeeComponent {
  @Input() employeeEmail: string | undefined;
  
}
