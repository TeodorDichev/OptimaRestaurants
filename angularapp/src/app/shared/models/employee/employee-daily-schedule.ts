import { Time } from "@angular/common";

export interface EmployeeDailySchedule {
    scheduleId: string,
    restaurantName: string,
    isWorkDay: boolean,
    isFullDay: boolean,
    from: Time,
    to: Time
}