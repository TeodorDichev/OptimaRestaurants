export interface ScheduleAssignment {
    scheduleId: string,
    employeeEmail: string,
    restaurantId: string,
    day: Date,
    from?: Date,
    to?: Date,
    isWorkDay: boolean,
    fullDay: boolean 
}