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

  getAllRestaurants(lastPageIndex: number) {
    return this.http.get(`${environment.appUrl}/api/restaurants/get-all-restaurants/${lastPageIndex}`);
  }

  getCitiesNames() {
    return this.http.get(`${environment.appUrl}/api/restaurants/get-city-names`);
  }

  getRestaurantsCountInACity(cityName: string) {
    return this.http.get(`${environment.appUrl}/api/restaurants/get-city-count/${cityName}`);
  }

  getAllRestaurantsInACity(lastPageIndex: number, cityName: string) {
    return this.http.get(`${environment.appUrl}/api/restaurants/get-city-restaurants/${cityName}/${lastPageIndex}`);
  }

  getBestCuisineRestaurants(lastPageIndex: number) {
    return this.http.get(`${environment.appUrl}/api/restaurants/get-cuisine-restaurants/${lastPageIndex}`);
  }

  getBestAtmosphereRestaurants(lastPageIndex: number) {
    return this.http.get(`${environment.appUrl}/api/restaurants/get-atmosphere-restaurants/${lastPageIndex}`);
  }

  getBestEmployeesRestaurants(lastPageIndex: number) {
    return this.http.get(`${environment.appUrl}/api/restaurants/get-employees-restaurants/${lastPageIndex}`);
  }

  getBestRestaurants(lastPageIndex: number) {
    return this.http.get(`${environment.appUrl}/api/restaurants/get-best-restaurants/${lastPageIndex}`);
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
