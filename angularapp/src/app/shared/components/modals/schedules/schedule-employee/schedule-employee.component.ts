import { Time } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Employee } from 'src/app/shared/models/employee/employee';
import { EmployeeDailySchedule } from 'src/app/shared/models/employee/employee-daily-schedule';
import { EmployeeFullSchedule } from 'src/app/shared/models/employee/employee-full-schedule';
import { ScheduleAssignment } from 'src/app/shared/models/employee/schedule-assignent';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';

@Component({
  selector: 'app-schedule-employee',
  templateUrl: './schedule-employee.component.html',
  styleUrls: ['./schedule-employee.component.css']
})
export class ScheduleEmployeeComponent implements OnInit {
  employee: Employee | undefined;

  currentDate: Date = new Date(); // calendar's current date on display
  daysInCurrentMonth: number = 0; // also the last date of the month
  firstDayOfMonth: number = 0; // gets what the first date's weekday is
  lastDayOfMonth: number = 0; // gets what the last date's weekday is
  week5FirstDay: number = 0;
  week5LastDay: number = 0;
  week6LastDay: number = 0;

  idMarkerPairs: Record<string, string> = {};
  today: Date = new Date();

  restaurantsNamesList: string[] = [];
  restaurantsIdsList: string[] = [];
  selectedRestaurantIndex: number = 0;

  dateMarkers = ['workday', 'offday', 'selected'];
  weekdays = ['Нед', 'Пон', 'Вто', 'Сря', 'Чет', 'Пет', 'Съб'];
  weekdaysFull = ['Неделя', 'Понеделник', 'Вторник', 'Сряда', 'Четвъртък', 'Петък', 'Събота'];
  monthsFull = ['Януари', 'Февруари', 'Март', 'Април', 'Май', 'Юни', 'Юли', 'Август', 'Септември', 'Октомври', 'Ноември', 'Декември'];

  selectedDay: Date = new Date(); // user's selected date, which could be on a different month/year than the one displayed
  selectedDayAsText: string = '';
  workDaysIds: string[] = [];

  selectedDaySchedule: EmployeeDailySchedule[] = [];
  scheduleAssignment: ScheduleAssignment = {
    scheduleId: '',
    employeeEmail: '',
    restaurantId: '',
    day: new Date(),
    fullDay: false,
    isWorkDay: false
  };
  employeeFullSchedule: EmployeeFullSchedule[] = [];

  fullDay: boolean = false;
  from: Time = { hours: 0, minutes: 0 };
  to: Time = { hours: 0, minutes: 0 };

  constructor(private emplopyeeService: EmployeeService,
    private bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    this.getEmployee();
    this.setTodayMarker();
    this.getRestaurantsNames();
    this.setUp();
    this.getDailySchedule();
  }

  setUp() {
    this.getSelectedDateAsText();
    this.setUpSchedule();
    this.setUpCalendarDisplay();
    this.setDatesClasses();
  }

  private setUpCalendarDisplay() {
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

  private setUpSchedule() {
    this.getRestaurantSchedule();
  }


  private getRestaurantsNames() {
    this.restaurantsNamesList.push('Всички');
    this.restaurantsIdsList.push('id-for-all-restaurants');

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
            console.log('full:', response);
          }
        })
      }
      else {
        this.emplopyeeService.getEmployeeRestaurantSchedule(this.employee.email, this.restaurantsIdsList[this.selectedRestaurantIndex], this.currentDate.getMonth())
          .subscribe({
            next: (response: any) => {
              console.log('rest:', response);
            }
          })
      }
    }
  }

  getDailySchedule() {
    if (this.employee) {
      this.emplopyeeService.getDailySchedule(this.employee.email, this.selectedDay).subscribe({
        next: (response: any) => {
          this.selectedDaySchedule = response;
        }
      })
    }
  }

  setDayToOffday() {
    if (this.employee) {
      if (this.fullDay) {
        this.scheduleAssignment = {
          scheduleId: this.selectedDaySchedule[0].scheduleId,
          restaurantId: this.restaurantsIdsList[this.selectedRestaurantIndex], //problem if he wants off day from all restaurants
          employeeEmail: this.employee?.email,
          day: this.selectedDay,
          isWorkDay: false,
          fullDay: this.fullDay
        }
      }
      else {
        this.scheduleAssignment = {
          scheduleId: this.selectedDaySchedule[0].scheduleId,
          restaurantId: this.restaurantsIdsList[this.selectedRestaurantIndex], //problem if he wants off day from all restaurants
          employeeEmail: this.employee?.email,
          day: this.selectedDay,
          from: this.from,
          to: this.to,
          isWorkDay: false,
          fullDay: this.fullDay
        }
      }
      this.emplopyeeService.addAssignment(this.scheduleAssignment).subscribe({
        next: (response: any) => {
          this.selectedDaySchedule = response;
        }
      })
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

  clearDatesClasses() {
    for (let id in this.idMarkerPairs) {
      for (let marker of this.dateMarkers) {
        document.getElementById(id)?.classList.remove(marker);
      }
    }
  }

  clearPreviousSelectedDateMarker() {
    for (let id in this.idMarkerPairs) {
      document.getElementById(id)?.classList.remove(this.dateMarkers[2]);
      let index = this.idMarkerPairs[id].indexOf(this.dateMarkers[2]);
      if (index !== -1) {
        this.idMarkerPairs[id] = this.idMarkerPairs[id].slice(0, index) + this.idMarkerPairs[id].slice(index + this.dateMarkers[2].length);
      }
    }
  }

  private setTodayMarker() {
    const todayId = this.today.getDate() + '_' + (this.today.getMonth() + 1) + '_' + this.currentDate.getFullYear();
    this.idMarkerPairs[todayId] = 'today ';
  }

  setDatesClasses() {
    this.clearPreviousSelectedDateMarker();

    const selectedDayId = this.selectedDay.getDate() + '_' + (this.selectedDay.getMonth() + 1) + '_' + this.selectedDay.getFullYear();
    if (this.idMarkerPairs[selectedDayId]) {
      this.idMarkerPairs[selectedDayId] += this.dateMarkers[2];
    }
    else {
      this.idMarkerPairs[selectedDayId] = this.dateMarkers[2];
    }
  }

  getId(date: number) {
    return date + '_' + (this.currentDate.getMonth() + 1) + '_' + this.currentDate.getFullYear();
  }

  selectDate(id: string) {
    this.selectedDay = new Date(parseInt(id.split('_')[2]), parseInt(id.split('_')[1]) - 1, parseInt(id.split('_')[0]));
    this.setDatesClasses();
    this.getSelectedDateAsText();
    this.getDailySchedule();
  }

  getSelectedDateAsText() {
    this.selectedDayAsText = `${this.weekdaysFull[this.selectedDay.getDay()]}, ${this.selectedDay.getDate()} ${this.monthsFull[this.selectedDay.getMonth()]}`
  }
}
