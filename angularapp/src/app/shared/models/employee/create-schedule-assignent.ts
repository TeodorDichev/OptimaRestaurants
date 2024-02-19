export interface CreateScheduleAssignment {
    employeeEmail: string,
    restaurantId: string,
    day: Date,
    from?: Date,
    to?: Date,
    isWorkDay: boolean,
    fullDay: boolean 
}