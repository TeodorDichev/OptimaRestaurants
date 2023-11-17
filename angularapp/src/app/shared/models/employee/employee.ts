import { Restraurant } from "../restaurant/restaurant";

export interface Employee{
    email: string,
    firstName: string,
    lastName: string,
    profilePictureUrl: string,
    phoneNumber: string,
    birthDate: Date,
    city: string,
    speedAverageRating: number,
    attitudeAverageRating: number,
    punctualityAverageRating: number,
    collegialityAverageRating: number,
    employeeAverageRating: number,
    isLookingForJob: boolean,
    restaurants: Restraurant[]
}