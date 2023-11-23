import { Component, OnInit } from '@angular/core';
import { RestaurantsService } from '../../pages/restaurants/restaurants.service';
import { Restaurant } from '../../models/restaurant/restaurant';
import { SharedService } from '../../shared.service';

@Component({
  selector: 'app-browse-all-restaurants',
  templateUrl: './browse-all-restaurants.component.html',
  styleUrls: ['./browse-all-restaurants.component.css',
    '../../../app.component.css']
})
export class BrowseAllRestaurantsComponent implements OnInit {

  allRestaurants: Restaurant[] = [];
  currentRestaurant: Restaurant | undefined;

  constructor(private sharedService: SharedService,
    private restaurantsService: RestaurantsService) { }

  ngOnInit(): void {
    this.getAllRestaurants();
  }

  getAllRestaurants() {
    this.restaurantsService.getAllRestaurants().subscribe({
      next: (response: any) => {
        this.allRestaurants = response;
        this.currentRestaurant = this.allRestaurants[0];
      }
    });;
  }

  getRestaurantDetails(restaurantId: string) {
    this.restaurantsService.getRestaurantDetails(restaurantId).subscribe({
      next: (response: any) => {
        this.sharedService.openRestaurantDetailsModal(response)
      }
    })
  }

  selectedRestaurant(selectedRestaurant: Restaurant) {
    this.currentRestaurant = selectedRestaurant;
  }

  missingIcon(restaurant: Restaurant) {
    restaurant.iconUrl = 'assets/images/logo-bw-with-bg.png';
  }
}
