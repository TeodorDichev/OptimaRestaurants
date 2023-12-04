export interface Request{
    id: string,
    text: string,
    senderEmail: string,
    restaurantId: string,
    sentOn: Date,
    confirmed: boolean
}