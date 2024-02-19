import { Component, OnDestroy, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CreateScheduleAssignment } from 'src/app/shared/models/employee/create-schedule-assignent';
import { ScheduleAssignment } from 'src/app/shared/models/employee/schedule-assignment';
import { Manager } from 'src/app/shared/models/manager/manager';
import { ManagerDailySchedule } from 'src/app/shared/models/manager/manager-daily-schedule';
import { ManagerFullSchedule } from 'src/app/shared/models/manager/manager-full-schedule';
import { SharedService } from 'src/app/shared/shared.service';
import { ManagerService } from './../../../../pages-routing/manager/manager.service';
import { FreeEmployee } from 'src/app/shared/models/manager/free-employee';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-schedule-manager',
  templateUrl: './schedule-manager.component.html',
  styleUrls: ['./schedule-manager.component.css']
})
export class ScheduleManagerComponent implements OnInit, OnDestroy {

  private subscriptions: Subscription[] = [];

  manager: Manager | undefined;

  currentDate: Date = new Date(); // calendar's current date on display
  daysInCurrentMonth: number = 0; // also the last date of the month
  firstDayOfMonth: number = 0; // gets what the first date's weekday is
  lastDayOfMonth: number = 0; // gets what the last date's weekday is
  week5FirstDay: number = 0;
  week5LastDay: number = 0;
  week6LastDay: number = 0;

  idMarkerPairs: Record<string, string> = {};
  today: Date = new Date();

  selectedRestaurantIndex: number = 0;

  dateMarkers = [ 'with-workers', 'selected'];
  weekdays = ['Нед', 'Пон', 'Вто', 'Сря', 'Чет', 'Пет', 'Съб'];
  weekdaysFull = ['Неделя', 'Понеделник', 'Вторник', 'Сряда', 'Четвъртък', 'Петък', 'Събота'];
  monthsFull = ['Януари', 'Февруари', 'Март', 'Април', 'Май', 'Юни', 'Юли', 'Август', 'Септември', 'Октомври', 'Ноември', 'Декември'];

  selectedDay: Date = new Date(); // user's selected date, which could be on a different month/year than the one displayed
  selectedDayAsText: string = '';
  workDaysIds: string[] = [];

  selectedDaySchedule: ManagerDailySchedule[] = [];
  managerFullSchedule: ManagerFullSchedule[] = [];
  createScheduleAssignment: CreateScheduleAssignment = {
    employeeEmail: '',
    restaurantId: '',
    day: new Date(),
    // from: '', to: ''
    fullDay: false,
    isWorkDay: false
  };
  selectedAssignment: ManagerDailySchedule = {
    scheduleId: '',
    employeeEmail: '',
    employeeName: '',
    // from: '', to: ''
    isWorkDay: false,
    isFullDay: false,
    restaurantName: ''
  };
  assignmentEdit: ScheduleAssignment = {
    scheduleId: '',
    restaurantId: '',
    // from: '', to: ''
    fullDay: false,
    isWorkDay: false,
    employeeEmail: '',
    day: new Date()
  };
  freeEmployeeList: FreeEmployee[] = []

  selectedEmployee: FreeEmployee = {
    employeeEmail: '',
    employeeName: '',
    restaurantName: ''
    // from: '', to: ''
  }

  fullDayForCreate: boolean = false;
  fromForCreate: string = '';
  toForCreate: string = '';
  workDayForCreate: boolean = false;

  fullDayForEdit: boolean = false;
  fromForEdit: string = '';
  toForEdit: string = '';
  workDayForEdit: boolean = false;

  isCreateCollapseOpen = false;
  isEditCollapseOpen = false;

  constructor(private managerService: ManagerService,
    private bsModalRef: BsModalRef,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getManager();
    this.setTodayMarker();
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

  private getManager() {
    const sub = this.managerService.manager$.subscribe({
      next: (response: any) => {
        this.manager = response;
      }
    });

    this.subscriptions.push(sub);
  }


  nextRestaurant() {
    if (this.manager) {
      if (this.selectedRestaurantIndex == this.manager?.restaurants.length - 1) {
        this.selectedRestaurantIndex = 0;
      }
      else {
        this.selectedRestaurantIndex++;
      }
      this.getRestaurantSchedule();
      this.getDailySchedule();
    }
  }

  previousRestaurant() {
    if (this.manager) {
      if (this.selectedRestaurantIndex == 0) {
        this.selectedRestaurantIndex = this.manager?.restaurants.length - 1;
      }
      else {
        this.selectedRestaurantIndex--;
      }
      this.getRestaurantSchedule();
      this.getDailySchedule();
    }
  }

  getRestaurantSchedule() {
    if (this.manager) {
      const sub = this.managerService.getManagerFullSchedule(this.manager.restaurants[this.selectedRestaurantIndex].id, this.selectedDay.getMonth() + 1).subscribe({
        next: (response: any) => {
          this.clearOnlyNoneAndWithWorkersMarkers();
          this.managerFullSchedule = response;
          this.setDatesMarkers();
        }
      });

      this.subscriptions.push(sub);
    }
  }

  getFreeEmployees() {
    if (this.manager) {
      const sub = this.managerService.getFreeEmployees(this.manager.restaurants[this.selectedRestaurantIndex].id, this.selectedDay).subscribe({
        next: (response: any) => {
          this.freeEmployeeList = response;
        }
      });
      this.subscriptions.push(sub);
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
    this.addMarkerToDate(selectedDayId, 1);
  }

  setDatesMarkers() {
    for (let schedule of this.managerFullSchedule) {
      const dateOnly = schedule.day.split('T');
      const normalizeDate = new Date(parseInt(dateOnly[0].split('-')[0]), parseInt(dateOnly[0].split('-')[1]) - 1, parseInt(schedule.day.split('T')[0].split('-')[2]));
      const currentId = this.getIdByDate(normalizeDate); // id for markers
      if (schedule.peopleAssignedToWork) {
        this.addMarkerToDate(currentId, 0); // add marker for no-workers
      }
    }
  }

  getDailySchedule() {
    this.resetScheduleAssignmentCreation();
    this.resetScheduleEdit();
    this.resetTimeRangeCreation();
    this.resetTimeRangeEdit();
    this.clearSelectedEmployee();
    if (this.manager) {
      const sub = this.managerService.getManagerDailySchedule(this.manager.restaurants[this.selectedRestaurantIndex].id, this.selectedDay).subscribe({
        next: (response: any) => {
          this.selectedDaySchedule = response;
        }
      });

      this.subscriptions.push(sub);
    }
  }

  private generateAssignment(restaurantId: string) {
    if (this.manager) {
      if (this.fullDayForCreate) {
        this.createScheduleAssignment = {
          restaurantId: restaurantId,
          employeeEmail: this.selectedEmployee.employeeEmail,
          day: this.selectedDay,
          isWorkDay: this.workDayForCreate,
          fullDay: this.fullDayForCreate
        }
      }
      else {
        this.createScheduleAssignment = {
          restaurantId: restaurantId,
          employeeEmail: this.selectedEmployee.employeeEmail,
          day: this.selectedDay,
          from: new Date(this.selectedDay.getFullYear(), this.selectedDay.getMonth(), this.selectedDay.getDate(), parseInt(this.fromForCreate.split(':')[0]), parseInt(this.fromForCreate.split(':')[1])),
          to: new Date(this.selectedDay.getFullYear(), this.selectedDay.getMonth(), this.selectedDay.getDate(), parseInt(this.toForCreate.split(':')[0]), parseInt(this.toForCreate.split(':')[1])),
          isWorkDay: this.workDayForCreate,
          fullDay: this.fullDayForCreate
        }
      }
    }
  }

  createAssignment() {
    if (this.manager && this.isCreateTimeRangeValid()) {
      this.generateAssignment(this.manager.restaurants[this.selectedRestaurantIndex].id);
      this.addAssignment();
    }
  }

  private addAssignment() {
    const sub = this.managerService.addAssignment(this.createScheduleAssignment).subscribe({
      next: (response: any) => {
        this.selectedDaySchedule = response;
        this.getRestaurantSchedule();
        this.sharedService
      }, error: error => {
        this.sharedService.showNotification(false, 'Неуспешно добавяне на ангажимент', error.error);
      }
    })
    this.resetTimeRangeCreation();
    this.resetScheduleAssignmentCreation();

    this.subscriptions.push(sub);
  }

  private resetTimeRangeCreation() {
    this.fullDayForCreate = false;
    this.fromForCreate = '';
    this.toForCreate = '';
    this.workDayForCreate = false;
  }

  private resetTimeRangeEdit() {
    this.fullDayForEdit = false;
    this.fromForEdit = '';
    this.toForEdit = '';
    this.workDayForEdit = false;
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
      employeeEmail: '',
      employeeName: '',
      // from: '', to: ''
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
      this.sharedService.showNotification(false, 'Невалидно време', 'Времевият интервал, който вие сте въвели е невалиден. Моля опитайте отново.');
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
      this.sharedService.showNotification(false, 'Невалидно време', 'Времевият интервал, който вие сте въвели е невалиден. Моля опитайте отново.');
    }
    return result;
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

  // this is for when we are changing months and want to keep the previous entries, example: not to lose today or selected date
  clearDatesClasses() {
    for (let id in this.idMarkerPairs) {
      for (let marker of this.dateMarkers) {
        document.getElementById(id)?.classList.remove(marker);
      }
    }
  }

  clearOnlyNoneAndWithWorkersMarkers() {
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
      let index = this.idMarkerPairs[id].indexOf(this.dateMarkers[1]);
      if (index !== -1) {
        this.idMarkerPairs[id] = this.idMarkerPairs[id].slice(0, index) + this.idMarkerPairs[id].slice(index + this.dateMarkers[1].length);
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

  selectAssignment(schedule: ManagerDailySchedule) {
    this.selectedAssignment = schedule;
  }

  private generateEditAssignment() {
    if (this.manager) {
      if (this.fullDayForEdit) {
        this.assignmentEdit = {
          scheduleId: this.selectedAssignment.scheduleId,
          restaurantId: this.manager.restaurants[this.selectedRestaurantIndex].id,
          employeeEmail: this.selectedAssignment.employeeEmail,
          day: this.selectedDay,
          isWorkDay: false,
          fullDay: this.fullDayForEdit
        }
      }
      else {
        this.assignmentEdit = {
          scheduleId: this.selectedAssignment.scheduleId,
          restaurantId: this.manager.restaurants[this.selectedRestaurantIndex].id,
          employeeEmail: this.selectedAssignment.employeeEmail,
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
      const sub = this.managerService.editAssignment(this.assignmentEdit).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, 'Успешно!', 'Вие успешно редактирахте този ангажимент.');
          this.selectedDaySchedule = response;
          this.resetTimeRangeEdit();
          this.resetScheduleEdit();
          this.getRestaurantSchedule();
        }, error: error => {
          this.resetTimeRangeEdit();
          this.resetScheduleEdit();
          this.isEditCollapseOpen = false;
          this.sharedService.showNotification(false, 'Грешка.', error.error)
        }
      });
      this.subscriptions.push(sub);
    }
  }

  deleteAssignment() {
    const sub = this.managerService.deleteAssignment(this.selectedAssignment.scheduleId).subscribe({
      next: (response: any) => {
        this.getRestaurantSchedule();
        this.getDailySchedule();
        this.sharedService.showNotification(true, response.value.title, response.value.message);
      }, error: error => {
        this.sharedService.showNotification(true, 'Неуспешно изтриване.', error.error);
      }
    });
    this.subscriptions.push(sub);
  }

  getSelectedDateAsText() {
    this.selectedDayAsText = `${this.weekdaysFull[this.selectedDay.getDay()]}, ${this.selectedDay.getDate()} ${this.monthsFull[this.selectedDay.getMonth()]}`
  }

  toggleCollapse(collapse: string) {
    this.freeEmployeeList = [];
    this.getFreeEmployees();
    if (collapse === 'createCollapse') {
      this.isCreateCollapseOpen = !this.isCreateCollapseOpen;
      this.isEditCollapseOpen = false;
    } else if (collapse === 'editCollapse') {
      this.isEditCollapseOpen = !this.isEditCollapseOpen;
      this.isCreateCollapseOpen = false;
    }
  }

  selectFreeEmployee(employee: FreeEmployee) {
    this.selectedEmployee = employee;
  }

  clearSelectedEmployee() {
    this.selectedEmployee = {
      employeeEmail: '',
      employeeName: '',
      restaurantName: '',
      from: undefined,
      to: undefined
    }
  }
}
