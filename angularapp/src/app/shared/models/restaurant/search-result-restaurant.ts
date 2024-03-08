export interface SearchResultRestaurant {
    id: string,
    name: string,
    address: string,
    city: string,
    icon: File,
    isWorking: boolean,
    restaurantAverageRating: number,
    totalReviewsCount: number
}