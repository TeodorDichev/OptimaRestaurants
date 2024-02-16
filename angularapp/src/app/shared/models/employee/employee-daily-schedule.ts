export interface EmployeeDailySchedule {
    scheduleId: string,
    restaurantName: string,
    restaurantId: string,
    isWorkDay: boolean,
    isFullDay: boolean,
    from?: string,
    to?: string
}