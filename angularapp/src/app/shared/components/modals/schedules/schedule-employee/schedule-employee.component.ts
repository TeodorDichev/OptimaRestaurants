import { BsModalRef } from 'ngx-bootstrap/modal';
import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Employee } from 'src/app/shared/models/employee/employee';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';

@Component({
  selector: 'app-schedule-employee',
  templateUrl: './schedule-employee.component.html',
  styleUrls: ['./schedule-employee.component.css']
})
export class ScheduleEmployeeComponent implements OnInit, AfterViewInit {
  employee: Employee | undefined;

  currentDate: Date = new Date();
  daysInCurrentMonth: number = 0;
  weekdays = ['Нед', 'Пон', 'Вто', 'Сря', 'Чет', 'Пет', 'Съб'];
  outsideMonthFlagsWeek3: boolean[] = [];
  outsideMonthFlagsWeek4: boolean[] = [];

  constructor(private emplopyeeService: EmployeeService,
    private bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.getEmployee();
    this.setUp();
  }

  ngAfterViewInit(): void {
    this.setSpecialDaysMarkers();
  }

  setElementId(weekNumber: number, day: number) {
    return weekNumber * 7 + day - this.getFirstWeekDay(this.currentDate);
  }

  setSpecialDaysMarkers() {
    const today = new Date();
    if (today) {
      document.getElementById(today.getDate().toString())?.classList.add('today');
    }
  }

  private setUp() {
    this.getDaysCountInCurrentMonth(this.currentDate);
    this.calculateOutsideMonthFlagsWeek3();
    this.calculateOutsideMonthFlagsWeek4();
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
    this.setUp();
    this.setSpecialDaysMarkers();
  }

  nextMonth() {
    this.currentDate.setMonth(this.currentDate.getMonth() + 1);
    this.setUp();
    this.setSpecialDaysMarkers();
  }

  calculateOutsideMonthFlagsWeek3() {
    this.outsideMonthFlagsWeek3 = [];
    for (let i = 0; i < 7; i++) {
      const dayNumber = this.getCurrentDay(3, i);
      this.outsideMonthFlagsWeek3.push(this.isInsideMonth(dayNumber));
    }
  }

  calculateOutsideMonthFlagsWeek4() {
    this.outsideMonthFlagsWeek4 = [];
    for (let i = 0; i < 7; i++) {
      const dayNumber = this.getCurrentDay(4, i);
      this.outsideMonthFlagsWeek4.push(this.isInsideMonth(dayNumber));
    }
  }

  isInsideMonth(dayNumber: number): boolean {
    return dayNumber <= this.daysInCurrentMonth;
  }

  test(dayNumber: number) {
    console.log('test-method-output', dayNumber);
  }

  getCurrentDay(weekNumber: number, dayOfWeekNumber: number) {
    return dayOfWeekNumber + 1 + 7 * weekNumber + this.getFirstWeekDay(this.currentDate);
  }
}
