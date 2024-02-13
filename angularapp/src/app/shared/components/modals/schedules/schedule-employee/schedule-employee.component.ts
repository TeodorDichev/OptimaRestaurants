import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
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
  daysInCurrentMonth: number = 0; // also the last date of the month
  firstDayOfMonth: number = 0; // gets what the first date's weekday is
  lastDayOfMonth: number = 0; // gets what the last date's weekday is
  week5FirstDay: number = 0;
  week5LastDay: number = 0;
  week6LastDay: number = 0;

  restaurantsNamesList: string[] = [];
  restaurantsIdsList: string[] = [];
  selectedRestaurantIndex: number = 0;

  markedDaysIds: string[] = [];
  dateMarkers = ['today', 'workday', 'offday', 'selected'];
  weekdays = ['Нед', 'Пон', 'Вто', 'Сря', 'Чет', 'Пет', 'Съб'];
  weekdaysFull = ['Неделя', 'Понеделник', 'Вторник', 'Сряда', 'Четвъртък', 'Петък', 'Събота'];
  monthsFull = ['Януари', 'Февруари', 'Март', 'Април', 'Май', 'Юни', 'Юли', 'Август', 'Септември', 'Октомври', 'Ноември', 'Декември'];

  selectedDay: Date = new Date();
  workDaysIds: string[] = [];

  constructor(private emplopyeeService: EmployeeService,
    private bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.getEmployee();
    this.getRestaurantsNames();
    this.setUp();
  }

  setUp() {
    this.setUpSchedule();
    this.setUpCalendarDisplay();
  }

  private setUpCalendarDisplay() {
    this.getDaysCountInCurrentMonth();
    this.getFirstWeekDay();
    this.getLastWeekDay();
    this.lastWeeksDays();
  }

  private setUpSchedule() {
    this.getRestaurantSchedule();
  }

  private getEmployee() {
    this.emplopyeeService.employee$.subscribe({
      next: (response: any) => {
        this.employee = response;
      }
    })
  }

  private getRestaurantsNames() {
    this.restaurantsNamesList.push('Всички');
    this.restaurantsIdsList.push('');

    if (this.employee?.restaurants) {
      for (let rest of this.employee.restaurants) {
        this.restaurantsNamesList.push(rest.name);
        this.restaurantsIdsList.push(rest.id);
      }
    }
  }

  nextRestaurant() {
    if (this.selectedRestaurantIndex == this.restaurantsIdsList.length - 1) {
      this.selectedRestaurantIndex = 0;
    }
    else {
      this.selectedRestaurantIndex++;
    }
    this.setUpSchedule();
  }

  previousRestaurant() {
    if (this.selectedRestaurantIndex == 0) {
      this.selectedRestaurantIndex = this.restaurantsNamesList.length - 1;
    }
    else {
      this.selectedRestaurantIndex--;
    }
    this.setUpSchedule();
  }

  getRestaurantSchedule() {
    if (this.employee) {
      if (this.selectedRestaurantIndex == 0) {
        this.emplopyeeService.getEmployeeFullSchedule(this.employee.email, this.currentDate.getMonth() + 1).subscribe({
          next: (response: any) => {
            //console.log(response);
          }
        })
      }
      else {
        this.emplopyeeService.getEmployeeRestaurantSchedule
          (this.employee.email, this.restaurantsIdsList[this.selectedRestaurantIndex], this.currentDate.getMonth()).subscribe({
            next: (response: any) => {
              //console.log(response);
            }
          })
      }

    }
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
    this.clearDatesClasses();
  }

  nextMonth() {
    this.currentDate.setMonth(this.currentDate.getMonth() + 1);
    this.setUp();
    this.clearDatesClasses();
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
      if (weekNumber == 4) return 7;
      if (weekNumber == 5) return differenceInWeek5 - 7;
    }
    else {
      if (weekNumber == 4) return differenceInWeek5;
      if (weekNumber == 5) return 0;
    }
    return 0;
  }

  // dateId: 1_2 -> date_month, class number in array -> 0-today, 1-workday, 2-offday
  markDate(dateId: string, classNumber: number) {
    try {
      console.log(document.getElementById(dateId));
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

  getClassForDate(id: string): string {
    const today = new Date();
    const todayId = today.getDate() + '_' + (today.getMonth() + 1);
    if (todayId == id && today.getFullYear() == this.currentDate.getFullYear()) {
      return this.dateMarkers[0];
    }
    else if (false) {
      // if it's a work day
    }
    else if (false) {
      // if it's a off day
    }
    return '';
  }

  getId(date: number) {
    return date + '_' + (this.currentDate.getMonth() + 1);
  }

  selectDate(id: string) {

  }

  getSelectedDate() {
    return `${this.weekdaysFull[this.selectedDay.getDay()]}, 
    ${this.selectedDay.getDate()} 
    ${this.monthsFull[this.selectedDay.getMonth()]}`
  }
}
