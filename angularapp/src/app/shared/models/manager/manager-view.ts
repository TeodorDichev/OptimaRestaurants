import { Restraurant } from "../restaurant/restaurant";

export interface ManagerView{
    email: string,
    firstName: string,
    lastName: string,
    profilePictureUrl: string,
    restaurants: Restraurant[]
}