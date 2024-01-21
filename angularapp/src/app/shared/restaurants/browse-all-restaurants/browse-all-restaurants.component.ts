import { Component, OnInit } from '@angular/core';
import { RestaurantsService } from '../../pages-routing/restaurants/restaurants.service';
import { Restaurant } from '../../models/restaurant/restaurant';
import { SharedService } from '../../shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-browse-all-restaurants',
  templateUrl: './browse-all-restaurants.component.html',
  styleUrls: ['./browse-all-restaurants.component.css']
})
export class BrowseAllRestaurantsComponent implements OnInit {

  filterInputForm: FormGroup = new FormGroup({});

  allRestaurants: Restaurant[] = [];
  totalRestaurantCount: number = 0;

  currentPage: number = 1;
  totalPages: number = 1;

  constructor(private sharedService: SharedService,
    private restaurantsService: RestaurantsService,
    private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.initializeForm();
    this.getRestaurantsCount();
    this.getAllRestaurants();
  }

  initializeForm() {
    this.filterInputForm = this.formBuilder.group({
      filter: ['', [Validators.required]]
    })
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
        if (response > 20) {
          this.totalPages = Math.round(response / 20) + response % 20;
          this.totalRestaurantCount = response;
        }
        else {
          this.totalRestaurantCount = response;
        }
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
    }
  }

  currentPageRest(pageNumber: number) {
    if (this.currentPage != pageNumber) {
      this.currentPage = pageNumber;
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage += 1;
    }
  }

  getRestaurantsByCity() {
    if (this.filterInputForm.valid) {
      this.currentPage = 1;
      this.restaurantsService.getAllRestaurantsInACity(this.currentPage, this.filterInputForm.get('filter')?.value).subscribe({
        next: (response: any) => {
          this.allRestaurants = response;
          this.filterInputForm.reset();
        }
      })
    }
  }

  getRestaurantsByRating() {
    if (this.filterInputForm.valid) {
      this.currentPage = 1;
      this.restaurantsService.getAllRestaurantsAboveRating(this.currentPage, this.filterInputForm.get('filter')?.value).subscribe({
        next: (response: any) => {
          this.allRestaurants = response;
        }
      })
    }
  }

  getBestCuisineRestaurants() {
    this.currentPage = 1;
    this.restaurantsService.getBestCuisineRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.allRestaurants = response;
      }
    })
  }

  getBestAtmosphereRestaurants() {
    this.currentPage = 1;
    this.restaurantsService.getBestAtmosphereRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.allRestaurants = response;
      }
    })
  }

  getBestEmployeesRestaurants() {
    this.currentPage = 1;
    this.restaurantsService.getBestEmployeesRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.allRestaurants = response;
      }
    })
  }

  getBestRestaurants() {
    this.currentPage = 1;
    this.restaurantsService.getBestRestaurants(this.currentPage).subscribe({
      next: (response: any) => {
        this.allRestaurants = response
      }
    })
  }
}
