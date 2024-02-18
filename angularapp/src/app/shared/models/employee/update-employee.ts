export interface UpdateEmployee {
    newFirstName: string,
    newLastName: string,
    newPhoneNumber: string,
    profilePictureFile: File,
    newBirthDate: Date,
    newCity: string,
    isLookingForJob: boolean,
    oldPassword: string,
    newPassword: string
}