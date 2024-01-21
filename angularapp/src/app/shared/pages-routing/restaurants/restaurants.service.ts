import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { EmployeeRequest } from '../../models/requests/employeeRequest';

@Injectable({
  providedIn: 'root'
})
export class RestaurantsService {

  constructor(private http: HttpClient) { }

  getAllRestaurantsCount() {
    return this.http.get(`${environment.appUrl}/api/restaurants/get-all-restaurants-count`);
  }

  /*
  add filter buttons in every restaurant-list instance ->
   by city, by rating avg, by rating user chooses (dropdown from 3 elements quis, empl, atmos)
   there is a method for all of the elements
   */

  getAllRestaurants(lastPageIndex: number) {
    return this.http.get(`${environment.appUrl}/api/restaurants/get-all-restaurants/${lastPageIndex}`);
  }

  getAllRestaurantsInACity(lastPageIndex: number, cityName: string) {
    return this.http.get(`${environment.appUrl}api/restaurants/get-local-restaurants/${cityName}/${lastPageIndex}`);
  }
  
  getAllRestaurantsAboveRating(lastPageIndex: number, rating: number) {
    return this.http.get(`${environment.appUrl}api/restaurants/get-rating-restaurants/${rating}/${lastPageIndex}`);
  }

  getBestCuisineRestaurants(lastPageIndex: number) {
    return this.http.get(`${environment.appUrl}api/restaurants/get-cuisine-restaurants/${lastPageIndex}`);
  }

  getBestAtmosphereRestaurants(lastPageIndex: number) {
    return this.http.get(`${environment.appUrl}api/restaurants/get-atmosphere-restaurants/${lastPageIndex}`);
  }

  getBestEmployeesRestaurants(lastPageIndex: number) {
    return this.http.get(`${environment.appUrl}api/restaurants/get-employees-restaurants/${lastPageIndex}`);
  }

  getRestaurantDetails(restaurantId: string) {
    return this.http.get(`${environment.appUrl}/api/restaurants/restaurant-details/${restaurantId}`);
  }

  sendWorkingRequest(employeeRequest: EmployeeRequest) {
    return this.http.post(`${environment.appUrl}/api/restaurants/send-working-request`, employeeRequest);
  }

  search(str: string) {
    return this.http.get(`${environment.appUrl}/api/restaurants/search/${str}`);
  }
}
