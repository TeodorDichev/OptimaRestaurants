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

  currentDate: Date = new Date();
  daysInCurrentMonth: number = 0;
  weekdays = ['Неделя', 'Понеделник', 'Вторник', 'Сряда', 'Четвъртък', 'Петък', 'Събота'];

  constructor(private emplopyeeService: EmployeeService,
    private bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.getEmployee();
    this.getDaysCountInCurrentMonth(this.currentDate);
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

  getFirstDayOfMonth(date: Date): number {
    return new Date(date.getFullYear(), date.getMonth(), 1).getDay();
  }

  getFirstWeekDay(date: Date) {
    return 7 - this.getFirstDayOfMonth(date);
  }

  getDaysCountInCurrentMonth(date: Date) {
    this.daysInCurrentMonth = new Date(date.getFullYear(), date.getMonth() + 1, 0).getDate();
  }

  previousMonth() {
    this.currentDate.setMonth(this.currentDate.getMonth() - 1);
    this.getDaysCountInCurrentMonth(this.currentDate);
  }

  nextMonth() {
    this.currentDate.setMonth(this.currentDate.getMonth() + 1);
    this.getDaysCountInCurrentMonth(this.currentDate);
  }

  isOutsideMonth(dayNumber: number) {
    if (dayNumber < this.daysInCurrentMonth) {
      document.getElementById(dayNumber.toString())?.classList.add('item-hover');
      // make onclick to execute test()
      return dayNumber;
    }
    return '-';
  }

  test() {
    console.log('asdasd')
  }
}
