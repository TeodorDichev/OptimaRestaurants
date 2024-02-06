import { Time } from "@angular/common";

export interface ScheduleAssignment {
    scheduleId: string,
    employeeEmail: string,
    restaurantId: string,
    day: Date,
    from: Time,
    to: Time,
    isWorkDay: boolean,
    fullDay: boolean 
}