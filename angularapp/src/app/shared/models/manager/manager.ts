import { Restaurant } from "../restaurant/restaurant";

export interface Manager{
    email: string,
    firstName: string,
    lastName: string,
    profilePictureUrl: string,
    restaurants: Restaurant[]
}