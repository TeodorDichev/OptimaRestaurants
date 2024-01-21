import { Component, OnInit } from '@angular/core';
import { RestaurantsService } from '../../pages-routing/restaurants/restaurants.service';
import { Restaurant } from '../../models/restaurant/restaurant';
import { SharedService } from '../../shared.service';

@Component({
  selector: 'app-browse-all-restaurants',
  templateUrl: './browse-all-restaurants.component.html',
  styleUrls: ['./browse-all-restaurants.component.css']
})
export class BrowseAllRestaurantsComponent implements OnInit {

  allRestaurants: Restaurant[] = [];
  totalRestaurantCount: number = 0;

  currentPage: number = 1;
  totalPages: number = 1;

  constructor(private sharedService: SharedService,
    private restaurantsService: RestaurantsService) { }

  ngOnInit(): void {
    this.getRestaurantsCount();
    this.getAllRestaurants();
  }

  getAllRestaurants() {
    this.restaurantsService.getAllRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.allRestaurants = response;
      }
    });
  }

  getRestaurantsCount() {
    this.restaurantsService.getAllRestaurantsCount().subscribe({
      next: (response: any) => {
        this.totalPages = Math.round(response / 20) + response % 20;
        this.totalRestaurantCount = response;
      }
    })
  }

  getRestaurantDetails(restaurantId: string) {
    this.sharedService.openRestaurantDetailsModal(restaurantId);
  }

  missingIcon(restaurant: Restaurant) {
    restaurant.iconPath = 'assets/images/logo-bw-with-bg.png';
  }

  previousPage() {
    if (this.currentPage != 1) {
      this.currentPage -= 1;
      this.restaurantsService.getAllRestaurants(this.currentPage).subscribe({
        next: (response: any) => {
          this.allRestaurants = response;
        }
      })
    }
  }

  getPageRestaurants(pageNumber: number) {
    if (this.currentPage != pageNumber) {
      this.currentPage = pageNumber;
      this.getAllRestaurants();  
    }
  }

  nextPage() { 
    if (this.currentPage < this.totalPages) {
      this.currentPage += 1;
      this.getAllRestaurants();
    }
  }
}
