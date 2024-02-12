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
  daysInCurrentMonth: number = 0; // also the last date of the month
  firstDayOfMonth: number = 0; // gets what the first date's weekday is
  lastDayOfMonth: number = 0; // gets what the last date's weekday is

  week5FirstDay: number = 0;
  week5LastDay: number = 0;
  week6LastDay: number = 0;

  markedDaysIds: string[] = [];

  dateMarkers = ['today', 'workday', 'offday'];
  weekdays = ['Нед', 'Пон', 'Вто', 'Сря', 'Чет', 'Пет', 'Съб'];

  constructor(private emplopyeeService: EmployeeService,
    private bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.getEmployee();
    this.setUp();
  }

  ngAfterViewInit() {
    this.setInitialMarkers();
  }

  private setUp() {
    this.getDaysCountInCurrentMonth();
    this.getFirstWeekDay();
    this.getLastWeekDay();
    this.lastWeeksDays();
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

  getFirstWeekDay() {
    this.firstDayOfMonth = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth(), 1).getDay();
  }

  getDaysCountInCurrentMonth() {
    this.daysInCurrentMonth = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() + 1, 0).getDate();
  }

  previousMonth() {
    this.currentDate.setMonth(this.currentDate.getMonth() - 1);
    this.setUp();
  }

  nextMonth() {
    this.currentDate.setMonth(this.currentDate.getMonth() + 1);
    this.setUp();
  }

  getDate(weekNumber: number, weekdayNumber: number) {
    return new Date(this.currentDate.getFullYear(), this.currentDate.getMonth(), weekdayNumber + (8 - this.firstDayOfMonth) + 7 * (weekNumber - 1)).getDate();
  }

  getLastWeekDay() {
    this.lastDayOfMonth = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() + 1, 0).getDay() + 1;
  }

  lastWeeksDays() {
    this.week5FirstDay = 7 - this.firstDayOfMonth + (5 - 2) * 7 + 1;
    this.week5LastDay = 6 + this.week5FirstDay;
  }

  getNumberOfDaysInWeek(weekNumber: number) {
    const differenceInWeek5 = this.daysInCurrentMonth - this.week5FirstDay + 1;
    if (differenceInWeek5 > 7) {
      if (weekNumber == 5) return 7;
      if (weekNumber == 6) return differenceInWeek5 - 7;
    }
    else {
      if (weekNumber == 5) return differenceInWeek5;
      if (weekNumber == 6) return 0;
    }
    return 0;
  }

  // dateId: 1_2 -> date_month, class number in array -> 0-today, 1-workday, 2-offday
  markDate(dateId: string, classNumber: number) {
    try {
      document.getElementById(dateId)?.classList.add(this.dateMarkers[classNumber]);
      this.markedDaysIds.push(dateId);
    } catch (error) { }
  }

  clearDatesClasses() {
    for (let id of this.markedDaysIds) {
      for (let marker of this.dateMarkers) {
        document.getElementById(id)?.classList.remove(marker);
      }
    }
  }

  setInitialMarkers() {
    const today = new Date();
    if (today.getFullYear() == this.currentDate.getFullYear()) {
      const todayId = today.getDate() + '_' + (today.getMonth() + 1);
      console.log(todayId);
      this.markDate(todayId, 0);  
    }
  }

  returnId(id: string) {
    console.log(id);
  }
}
