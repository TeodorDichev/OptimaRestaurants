import { Component, OnDestroy, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Employee } from 'src/app/shared/models/employee/employee';
import { EmployeeDailySchedule } from 'src/app/shared/models/employee/employee-daily-schedule';
import { EmployeeFullSchedule } from 'src/app/shared/models/employee/employee-full-schedule';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';
import { SharedService } from 'src/app/shared/shared.service';
import { CreateScheduleAssignment } from './../../../../models/employee/create-schedule-assignent';
import { ScheduleAssignment } from 'src/app/shared/models/employee/schedule-assignment';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-schedule-employee',
  templateUrl: './schedule-employee.component.html',
  styleUrls: ['./schedule-employee.component.css']
})
export class ScheduleEmployeeComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];
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
  employeeSchedule: EmployeeFullSchedule[] = [];
  createScheduleAssignment: CreateScheduleAssignment = {
    employeeEmail: '',
    restaurantId: '',
    day: new Date(),
    fullDay: false,
    isWorkDay: false
  };
  selectedAssignment: EmployeeDailySchedule = {
    scheduleId: '',
    restaurantId: '',
    isWorkDay: false,
    isFullDay: false,
    restaurantName: ''
  };
  assignmentEdit: ScheduleAssignment = {
    scheduleId: '',
    restaurantId: '',
    fullDay: false,
    isWorkDay: false,
    employeeEmail: '',
    day: new Date()
  };

  fullDayForCreate: boolean = false;
  fromForCreate: string = '';
  toForCreate: string = '';

  fullDayForEdit: boolean = false;
  fromForEdit: string = '';
  toForEdit: string = '';

  errorMessages: string[] = [];
  successSend: boolean = false;

  isCreateCollapseOpen = false;
  isEditCollapseOpen = false;

  constructor(private emplopyeeService: EmployeeService,
    private bsModalRef: BsModalRef,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getEmployee();
    this.setTodayMarker();
    this.getRestaurantsNames();
    this.setUp();
    this.setSelectedMarker();
    this.getDailySchedule();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  setUp() {
    this.getSelectedDateAsText();
    this.getRestaurantSchedule();
    this.setUpCalendarDisplay();
  }

  private setUpCalendarDisplay() {
    this.getDaysCountInCurrentMonth();
    this.getFirstWeekDay();
    this.getLastWeekDay();
    this.lastWeeksDays();
  }

  private getEmployee() {
    const sub = this.emplopyeeService.employee$.subscribe({
      next: (response: any) => {
        this.employee = response;
      }
    });
    this.subscriptions.push(sub);
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
    this.getRestaurantSchedule();
  }

  previousRestaurant() {
    if (this.selectedRestaurantIndex == 0) {
      this.selectedRestaurantIndex = this.restaurantsNamesList.length - 1;
    }
    else {
      this.selectedRestaurantIndex--;
    }
    this.getRestaurantSchedule();
  }

  getRestaurantSchedule() {
    if (this.employee) {
      if (this.selectedRestaurantIndex == 0) {
        this.emplopyeeService.getEmployeeFullSchedule(this.employee.email, this.currentDate.getMonth() + 1).subscribe({
          next: (response: any) => {
            this.clearOnlyWorkAndOffDaysMarkers();
            this.employeeSchedule = response;
            this.setDatesMarkers();
          }
        })
      }
      else {
        this.emplopyeeService.getEmployeeRestaurantSchedule(this.employee.email, this.restaurantsIdsList[this.selectedRestaurantIndex], this.currentDate.getMonth() + 1)
          .subscribe({
            next: (response: any) => {
              this.clearOnlyWorkAndOffDaysMarkers();
              this.employeeSchedule = response;
              this.setDatesMarkers();
            }
          })
      }
    }
  }

  private addMarkerToDate(id: string, markerIndex: number) {
    if (this.idMarkerPairs[id]) {
      if (!this.idMarkerPairs[id].includes(this.dateMarkers[markerIndex]))
        this.idMarkerPairs[id] += this.dateMarkers[markerIndex] + ' ';
    }
    else {
      this.idMarkerPairs[id] = this.dateMarkers[markerIndex] + ' ';
    }
  }

  setSelectedMarker() {
    this.clearPreviousSelectedDateMarker();
    const selectedDayId = this.getIdByDate(this.selectedDay);
    this.addMarkerToDate(selectedDayId, 2);
  }

  setDatesMarkers() {
    for (let schedule of this.employeeSchedule) {
      const dateOnly = schedule.day.split('T');
      const normalizeDate = new Date(parseInt(dateOnly[0].split('-')[0]), parseInt(dateOnly[0].split('-')[1]) - 1, parseInt(schedule.day.split('T')[0].split('-')[2]));
      const currentId = this.getIdByDate(normalizeDate); // id for markers
      if (schedule.isWorkDay) {
        this.addMarkerToDate(currentId, 0); // add marker for workday
      }
      else {
        this.addMarkerToDate(currentId, 1); // add marker for offday
      }
    }
  }

  getDailySchedule() {
    this.resetScheduleAssignmentCreation();
    this.resetScheduleEdit();
    this.resetTimeRangeCreation();
    this.resetTimeRangeEdit();
    if (this.employee) {
      const sub = this.emplopyeeService.getDailySchedule(this.employee.email, this.selectedDay).subscribe({
        next: (response: any) => {
          this.selectedDaySchedule = response;
        }
      });
      this.subscriptions.push(sub);
    }
  }

  private generateOffdayAssignment(restaurantId: string) {
    if (this.employee) {
      if (this.fullDayForCreate) {
        this.createScheduleAssignment = {
          restaurantId: restaurantId,
          employeeEmail: this.employee?.email,
          day: this.selectedDay,
          isWorkDay: false,
          fullDay: this.fullDayForCreate
        }
      }
      else {
        this.createScheduleAssignment = {
          restaurantId: restaurantId,
          employeeEmail: this.employee?.email,
          day: this.selectedDay,
          from: new Date(this.selectedDay.getFullYear(), this.selectedDay.getMonth(), this.selectedDay.getDate(), parseInt(this.fromForCreate.split(':')[0]), parseInt(this.fromForCreate.split(':')[1])),
          to: new Date(this.selectedDay.getFullYear(), this.selectedDay.getMonth(), this.selectedDay.getDate(), parseInt(this.toForCreate.split(':')[0]), parseInt(this.toForCreate.split(':')[1])),
          isWorkDay: false,
          fullDay: this.fullDayForCreate
        }
      }
    }
  }

  setDayToOffday() {
    this.errorMessages = [];
    if (this.employee && this.isCreateTimeRangeValid()) {
      if (this.restaurantsIdsList[this.selectedRestaurantIndex] == 'id-for-all-restaurants') {
        for (let restaurant of this.employee.restaurants) {
          this.generateOffdayAssignment(restaurant.id);
          this.addAssignment();
        }
        this.resetTimeRangeCreation();
        this.resetScheduleAssignmentCreation();
      }
      else {
        this.generateOffdayAssignment(this.restaurantsIdsList[this.selectedRestaurantIndex]);
        this.addAssignment();
        this.resetTimeRangeCreation();
        this.resetScheduleAssignmentCreation();
      }
      if (this.successSend) {
        this.sharedService.showNotification(true, 'Успешно записан ангажимент!', 'Вашият ангажимент беше успешно записан, може спокойно да продължите работа.');
      }
    }
  }

  private resetTimeRangeCreation() {
    this.fullDayForCreate = false;
    this.fromForCreate = '';
    this.toForCreate = '';
  }

  private resetTimeRangeEdit() {
    this.fullDayForEdit = false;
    this.fromForEdit = '';
    this.toForEdit = '';
  }

  private resetScheduleAssignmentCreation() {
    this.createScheduleAssignment = {
      employeeEmail: '',
      restaurantId: '',
      day: new Date(),
      from: new Date(),
      to: new Date(),
      fullDay: false,
      isWorkDay: false
    };
    this.isCreateCollapseOpen = false;
  }

  private resetScheduleEdit() {
    this.assignmentEdit = {
      scheduleId: '',
      restaurantId: '',
      from: new Date(),
      to: new Date(),
      fullDay: false,
      isWorkDay: false,
      employeeEmail: '',
      day: new Date()
    };
    this.selectedAssignment = {
      scheduleId: '',
      restaurantId: '',
      from: '',
      to: '',
      isWorkDay: false,
      isFullDay: false,
      restaurantName: ''
    };
    this.isEditCollapseOpen = false;
  }

  private isCreateTimeRangeValid(): boolean {
    if (this.fullDayForCreate) return true;
    const fromHours = parseInt(this.fromForCreate.split(':')[0]);
    const fromMinutes = parseInt(this.fromForCreate.split(':')[1]);
    const toHours = parseInt(this.toForCreate.split(':')[0]);
    const toMinutes = parseInt(this.toForCreate.split(':')[1]);

    const result = fromHours < toHours || (fromHours === toHours && fromMinutes < toMinutes);

    if (result == false) {
      this.errorMessages.push('Невалиден времеви интервал.');
      this.successSend = false;
    }
    return result;
  }

  private isEditTimeRangeValid(): boolean {
    if (this.fullDayForEdit) return true;
    const fromHours = parseInt(this.fromForEdit.split(':')[0]);
    const fromMinutes = parseInt(this.fromForEdit.split(':')[1]);
    const toHours = parseInt(this.toForEdit.split(':')[0]);
    const toMinutes = parseInt(this.toForEdit.split(':')[1]);

    const result = fromHours < toHours || (fromHours === toHours && fromMinutes < toMinutes);

    if (result == false) {
      this.errorMessages.push('Невалиден времеви интервал.');
      this.successSend = false;
    }
    return result;
  }

  private addAssignment() {
    const sub = this.emplopyeeService.addAssignment(this.createScheduleAssignment).subscribe({
      next: (response: any) => {
        this.errorMessages = [];
        this.successSend = true;
        this.selectedDaySchedule = response;
        this.getRestaurantSchedule();
      }, error: error => {
        this.successSend = false;
        this.errorMessages.push(error.error);
      }
    });
    this.subscriptions.push(sub);
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

  clearOnlyWorkAndOffDaysMarkers() {
    for (let id in this.idMarkerPairs) {
      for (let marker of this.dateMarkers.slice(0, this.dateMarkers.length - 1)) {
        let index = this.idMarkerPairs[id].indexOf(marker);
        if (index !== -1) {
          this.idMarkerPairs[id] = this.idMarkerPairs[id].slice(0, index) + this.idMarkerPairs[id].slice(index + marker.length);
        }
      }
    }
  }

  clearPreviousSelectedDateMarker() {
    for (let id in this.idMarkerPairs) {
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

  getIdByDate(date: Date) {
    return date.getDate() + '_' + (date.getMonth() + 1) + '_' + date.getFullYear();
  }

  getId(date: number) {
    return date + '_' + (this.currentDate.getMonth() + 1) + '_' + this.currentDate.getFullYear();
  }

  selectDate(id: string) {
    this.selectedDay = new Date(parseInt(id.split('_')[2]), parseInt(id.split('_')[1]) - 1, parseInt(id.split('_')[0]));
    this.setSelectedMarker();
    this.getSelectedDateAsText();
    this.getDailySchedule();
  }

  selectAssignment(schedule: EmployeeDailySchedule) {
    this.selectedAssignment = schedule;
  }

  private generateEditAssignment() {
    if (this.employee) {
      if (this.fullDayForEdit) {
        this.assignmentEdit = {
          scheduleId: this.selectedAssignment.scheduleId,
          restaurantId: this.selectedAssignment.restaurantId,
          employeeEmail: this.employee?.email,
          day: this.selectedDay,
          isWorkDay: false,
          fullDay: this.fullDayForEdit
        }
      }
      else {
        this.assignmentEdit = {
          scheduleId: this.selectedAssignment.scheduleId,
          restaurantId: this.selectedAssignment.restaurantId,
          employeeEmail: this.employee?.email,
          day: this.selectedDay,
          from: new Date(this.selectedDay.getFullYear(), this.selectedDay.getMonth(), this.selectedDay.getDate(), parseInt(this.fromForEdit.split(':')[0]), parseInt(this.fromForEdit.split(':')[1])),
          to: new Date(this.selectedDay.getFullYear(), this.selectedDay.getMonth(), this.selectedDay.getDate(), parseInt(this.toForEdit.split(':')[0]), parseInt(this.toForEdit.split(':')[1])),
          isWorkDay: false,
          fullDay: this.fullDayForEdit
        }
      }
    }
  }

  editAssignment() {
    if (this.isEditTimeRangeValid()) {
      this.generateEditAssignment();
      const sub = this.emplopyeeService.editAssignment(this.assignmentEdit).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, 'Успешно!', 'Вие успешно редактирахте този ангажимент.')
          this.selectedDaySchedule = response;
          this.resetTimeRangeEdit();
          this.resetScheduleEdit();
          this.getRestaurantSchedule();
        }, error: error => {
          this.resetTimeRangeEdit();
          this.resetScheduleEdit();
          this.isEditCollapseOpen = false;
          this.sharedService.showNotification(false, 'Грешка', error.error);
        }
      });
      this.subscriptions.push(sub);
    }
  }

  deleteAssignment() {
    const sub = this.emplopyeeService.deleteAssignment(this.selectedAssignment.scheduleId).subscribe({
      next: (response: any) => {
        this.getRestaurantSchedule();
        this.getDailySchedule();
        this.resetTimeRangeEdit();
        this.resetScheduleEdit();
        this.isEditCollapseOpen = false;
        this.sharedService.showNotification(true, response.value.title, response.value.message)
      }, error: error => {
        this.resetTimeRangeEdit();
        this.resetScheduleEdit();
        this.isEditCollapseOpen = false;
        this.sharedService.showNotification(true, 'Неуспешно изтриване.', error.error);
      }
    });
    this.subscriptions.push(sub);
  }

  getSelectedDateAsText() {
    this.selectedDayAsText = `${this.weekdaysFull[this.selectedDay.getDay()]}, ${this.selectedDay.getDate()} ${this.monthsFull[this.selectedDay.getMonth()]}`
  }

  toggleCollapse(collapse: string) {
    if (collapse === 'createCollapse') {
      this.isCreateCollapseOpen = !this.isCreateCollapseOpen;
      this.isEditCollapseOpen = false;
    } else if (collapse === 'editCollapse') {
      this.isEditCollapseOpen = !this.isEditCollapseOpen;
      this.isCreateCollapseOpen = false;
    }
  }
}
