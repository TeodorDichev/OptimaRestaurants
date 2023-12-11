import { Restaurant } from "../restaurant/restaurant";

export interface Employee{
    email: string,
    firstName: string,
    lastName: string,
    profilePicturePath: string,
    phoneNumber: string,
    birthDate: Date,
    city: string,
    QrCodePath: string,
    ResumePath: string,
    speedAverageRating: number,
    attitudeAverageRating: number,
    punctualityAverageRating: number,
    collegialityAverageRating: number,
    employeeAverageRating: number,
    isLookingForJob: boolean,
    restaurants: Restaurant[],
    qrCodePath: string
}