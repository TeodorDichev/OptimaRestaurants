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

  filterCurrent: string = '';
  cities: string[] = [];
  currentCity: string | undefined;

  allRestaurants: Restaurant[] = [];
  totalRestaurantCount: number = 0;

  currentPage: number = 1;
  totalPages: number = 1;

  constructor(private sharedService: SharedService,
    private restaurantsService: RestaurantsService) { }

  ngOnInit(): void {
    this.getCitiesNames();
    this.getAllRestaurants();
  }

  getAllRestaurants() {
    this.getAllRestaurantsCount();
    this.restaurantsService.getAllRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.filterCurrent = '';
        this.allRestaurants = response;
      }
    });
  }

  getAllRestaurantsCount() {
    this.restaurantsService.getAllRestaurantsCount().subscribe({
      next: (response: any) => {
        this.totalPages = Math.ceil(response / 20);
        this.totalRestaurantCount = response;
      }
    })
  }

  getCitiesNames() {
    this.restaurantsService.getCitiesNames().subscribe({
      next: (response: any) => {
        this.cities = response;
      }
    })
  }

  getRestaurantsCountInACity(cityName: string) {
    this.restaurantsService.getRestaurantsCountInACity(cityName).subscribe({
      next: (response: any) => {
        this.totalPages = Math.ceil(response / 20);
        this.totalRestaurantCount = response;
      }
    })
  }

  getRestaurantsByCity(cityName: string) {
    this.getRestaurantsCountInACity(cityName);
    this.restaurantsService.getAllRestaurantsInACity(this.currentPage, cityName).subscribe({
      next: (response: any) => {
        this.filterCurrent = 'Включен филтър за градове';
        this.currentCity = cityName;
        this.allRestaurants = response;
      }
    })
  }

  getBestCuisineRestaurants() {
    this.getAllRestaurantsCount();
    this.restaurantsService.getBestCuisineRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.filterCurrent = 'Включено сортиране по ястия';
        this.allRestaurants = response;
      }
    })
  }

  getBestAtmosphereRestaurants() {
    this.getAllRestaurantsCount();
    this.restaurantsService.getBestAtmosphereRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.filterCurrent = 'Включено сортиране по атмосфера';
        this.allRestaurants = response;
      }
    })
  }

  getBestEmployeesRestaurants() {
    this.getAllRestaurantsCount();
    this.restaurantsService.getBestEmployeesRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.filterCurrent = 'Включено сортиране по работници';
        this.allRestaurants = response;
      }
    })
  }

  getBestRestaurants() {
    this.getAllRestaurantsCount();
    this.restaurantsService.getBestRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.filterCurrent = 'Включено сортиране по ресторанти';
        this.allRestaurants = response;
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
      this.changePage();
    }
  }

  currentPageRest(pageNumber: number) {
    if (this.currentPage != pageNumber) {
      this.currentPage = pageNumber;
      this.changePage();
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage += 1;
      this.changePage();
    }
  }

  private changePage() {
    if (this.filterCurrent == '') {
      this.getAllRestaurants();
    }
    else if (this.filterCurrent == 'Включен филтър за градове' && this.currentCity) {
      this.getRestaurantsByCity(this.currentCity);
    }
    else if (this.filterCurrent == 'Включено сортиране по атмосфера') {
      this.getBestCuisineRestaurants();
    }
    else if (this.filterCurrent == 'Включено сортиране по атмосфера') {
      this.getBestAtmosphereRestaurants();
    }
    else if (this.filterCurrent == 'Включено сортиране по работници') {
      this.getBestEmployeesRestaurants();
    }
    else if (this.filterCurrent == 'Включено сортиране по ресторанти') {
      this.getBestRestaurants();
    }
  }

  setPageToFirst() {
    this.currentPage = 1;
  }
}
