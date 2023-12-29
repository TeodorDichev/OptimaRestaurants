import { Restaurant } from "../restaurant/restaurant";

export interface Manager{
    email: string,
    firstName: string,
    lastName: string,
    profilePicturePath: string,
    phoneNumber: string, 
    restaurants: Restaurant[]
}