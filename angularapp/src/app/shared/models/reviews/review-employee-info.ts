import { Restaurant } from "../restaurant/restaurant";

export interface ReviewEmployeeInfo {
    employeeEmail: string,
    employeeName: string,
    restaurants: Restaurant[]
}