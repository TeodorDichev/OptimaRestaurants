import { Component } from '@angular/core';
import { Employee } from '../../models/employee/employee';

@Component({
  selector: 'app-employees-looking-for-job',
  templateUrl: './employees-looking-for-job.component.html',
  styleUrls: ['./employees-looking-for-job.component.css']
})
export class EmployeesLookingForJobComponent {
  employeesLookingForJob: Employee[] = [];
}
