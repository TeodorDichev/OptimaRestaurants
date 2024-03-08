import { Component, OnDestroy, OnInit } from '@angular/core';
import { RestaurantsService } from '../../pages-routing/restaurants/restaurants.service';
import { Restaurant } from '../../models/restaurant/restaurant';
import { SharedService } from '../../shared.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-browse-all-restaurants',
  templateUrl: './browse-all-restaurants.component.html',
  styleUrls: ['./browse-all-restaurants.component.css']
})
export class BrowseAllRestaurantsComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

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

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  getAllRestaurants() {
    this.getAllRestaurantsCount();
    const sub = this.restaurantsService.getAllRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.filterCurrent = '';
        this.allRestaurants = response;
      }
    });
    this.subscriptions.push(sub);
  }

  getAllRestaurantsCount() {
    const sub = this.restaurantsService.getAllRestaurantsCount().subscribe({
      next: (response: any) => {
        this.totalPages = Math.ceil(response / 20);
        this.totalRestaurantCount = response;
      }
    });
    this.subscriptions.push(sub);
  }

  getCitiesNames() {
    const sub = this.restaurantsService.getCitiesNames().subscribe({
      next: (response: any) => {
        this.cities = response;
      }
    });
    this.subscriptions.push(sub);
  }

  getRestaurantsCountInACity(cityName: string) {
    const sub = this.restaurantsService.getRestaurantsCountInACity(cityName).subscribe({
      next: (response: any) => {
        this.totalPages = Math.ceil(response / 20);
        this.totalRestaurantCount = response;
      }
    });
    this.subscriptions.push(sub);
  }

  getRestaurantsByCity(cityName: string) {
    this.getRestaurantsCountInACity(cityName);
    const sub = this.restaurantsService.getAllRestaurantsInACity(this.currentPage, cityName).subscribe({
      next: (response: any) => {
        this.filterCurrent = 'Включен филтър за градове';
        this.currentCity = cityName;
        this.allRestaurants = response;
      }
    });
    this.subscriptions.push(sub);
  }

  getBestCuisineRestaurants() {
    this.getAllRestaurantsCount();
    const sub = this.restaurantsService.getBestCuisineRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.filterCurrent = 'Включено сортиране по ястия';
        this.allRestaurants = response;
      }
    });
    this.subscriptions.push(sub);
  }

  getBestAtmosphereRestaurants() {
    this.getAllRestaurantsCount();
    const sub = this.restaurantsService.getBestAtmosphereRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.filterCurrent = 'Включено сортиране по атмосфера';
        this.allRestaurants = response;
      }
    });
    this.subscriptions.push(sub);
  }

  getBestEmployeesRestaurants() {
    this.getAllRestaurantsCount();
    const sub = this.restaurantsService.getBestEmployeesRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.filterCurrent = 'Включено сортиране по работници';
        this.allRestaurants = response;
      }
    });
    this.subscriptions.push(sub);
  }

  getBestRestaurants() {
    this.getAllRestaurantsCount();
    const sub = this.restaurantsService.getBestRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.filterCurrent = 'Включено сортиране по ресторанти';
        this.allRestaurants = response;
      }
    });
    this.subscriptions.push(sub);
  }


  getRestaurantDetails(restaurantId: string) {
    this.sharedService.openRestaurantDetailsModal(restaurantId);
  }

  missingIcon(restaurant: Restaurant) {
    //restaurant.iconPath = 'assets/images/logo-bw-with-bg.png';
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
