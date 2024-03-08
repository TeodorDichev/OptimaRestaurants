import { Restaurant } from "../restaurant/restaurant";

export interface Employee{
    email: string,
    firstName: string,
    lastName: string,
    profilePicture: File,
    phoneNumber: string,
    birthDate: Date,
    city: string,
    resumePath: string,
    speedAverageRating: number,
    attitudeAverageRating: number,
    punctualityAverageRating: number,
    collegialityAverageRating: number,
    employeeAverageRating: number,
    isLookingForJob: boolean,
    restaurants: Restaurant[],
    qrCodePath: string
}