export interface ManagerDailySchedule {
    scheduleId: string,
    employeeEmail: string,
    employeeName: string,
    restaurantName: string,
    isFullDay: boolean,
    isWorkDay: boolean,
    from?: string,
    to?: string
}