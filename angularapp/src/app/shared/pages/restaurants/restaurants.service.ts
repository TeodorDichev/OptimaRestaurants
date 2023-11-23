import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class RestaurantsService {

  constructor(private http: HttpClient) { }

  getAllRestaurants() {
    return this.http.get(`${environment.appUrl}/api/restaurants/get-all-restaurants`);
  }

  getRestaurantDetails(restaurantId: string) {
    return this.http.get(`${environment.appUrl}/api/restaurants/restaurant-details/${restaurantId}`);
  }

}
