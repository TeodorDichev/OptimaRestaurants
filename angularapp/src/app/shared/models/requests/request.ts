export interface Request{
    id: string,
    text: string,
    senderEmail: string,
    restaurantName: string,
    sentOn: Date,
    confirmed: boolean
}