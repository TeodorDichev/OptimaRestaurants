export interface Restaurant {
    id: string,
    name: string
    address: string,
    city: string,
    employeeCapacity: number,
    isWorking: boolean,
    iconPath: string,
    cuisineAverageRating: number,
    atmosphereAverageRating: number,
    employeesAverageRating: number,
    restaurantAverageRating: number,
    managerFullName: string,
    managerEmail: string,
    managerPhoneNumber: string,
    topEmployeeFullName: string,
    topEmployeeEmail: string,
    topEmployeeRating: number,
    topEmployeePicturePath: string,
    totalReviewsCount: number
}