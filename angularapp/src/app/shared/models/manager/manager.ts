import { Restaurant } from "../restaurant/restaurant";

export interface Manager{
    email: string,
    firstName: string,
    lastName: string,
    profilePicture: File,
    phoneNumber: string, 
    restaurants: Restaurant[]
}