export interface UpdateManager{
    oldEmail: string,
    oldFirstName: string,
    oldLastName: string,
    oldPhoneNumber: number,
    oldPictureUrl: string,

    newEmail: string,
    newFirstName: string,
    newLastName: string,
    newPhoneNumber: number,
    newPassword: string,
    newPictureUrl: string
}