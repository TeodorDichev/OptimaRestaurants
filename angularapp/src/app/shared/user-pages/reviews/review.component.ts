import { ManagerReview } from './../../models/reviews/manager-review';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Restaurant } from '../../models/restaurant/restaurant';
import { ReviewEmployeeInfo } from '../../models/reviews/review-employee-info';
import { ReviewsService } from '../../pages-routing/review/reviews.service';
import { SharedService } from '../../shared.service';
import { CustomerReview } from '../../models/reviews/customer-review';
import { AccountService } from '../../pages-routing/account/account.service';

@Component({
  selector: 'app-review',
  templateUrl: './review.component.html',
  styleUrls: ['./review.component.css']
})
export class ReviewsComponent implements OnInit {
  errorMessages: string[] = [];
  reviewEmployeeInfo: ReviewEmployeeInfo | undefined;
  selectedRestaurant: Restaurant | undefined;

  customerReview: CustomerReview = {
    restaurantId: '',
    employeeEmail: '',
    comment: '',
    speedRating: 0,
    attitudeRating: 0,
    atmosphereRating: 0,
    cuisineRating: 0
  }

  constructor(private reviewsService: ReviewsService,
    private sharedService: SharedService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private accountService: AccountService) { }

  ngOnInit(): void {
    this.authorizeRequest();
  }

  private authorizeRequest() {
    if (this.accountService.getIsManager() === false) {
      this.router.navigateByUrl('');
    }
    else {
      this.activatedRoute.queryParamMap.subscribe({
        next: (params: any) => {
          this.getCustomerReviewForm(params.get('email'), params.get('token'));
        }
      })
    }
  }

  getCustomerReviewForm(email: string, token: string) {
    if (token && email) {
      this.reviewsService.getCustomerReviewForm(email, token).subscribe({
        next: (response: any) => {
          this.reviewEmployeeInfo = response;
        }, error: error => {
          //this.router.navigateByUrl('');
          this.sharedService.showNotification(false, 'Невалиден токен!', error.error);
        }
      });
    } else {
      //this.router.navigateByUrl('');
      this.sharedService.showNotification(false, 'Липсващ токен!', 'Моля проверете дали сте сканирали правилно QR кода.');
    }
  }

  submitCustomerReview() {
    if (this.reviewEmployeeInfo && this.selectedRestaurant) {
      this.customerReview.restaurantId = this.selectedRestaurant.id;
      this.customerReview.employeeEmail = this.reviewEmployeeInfo.employeeEmail;
    }

    if (this.customerReview.comment.length > 300) {
      this.sharedService.showNotification(false, 'Твърде дълго съобщение!', 'Вашето съобщение трябва да се състои от максимум 300 символа.')
    }
    else {
      this.reviewsService.submitCustomerReview(this.customerReview).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.router.navigateByUrl('');
        }
      })
    }
  }

  getSpeedRating(rating: number) {
    this.customerReview.speedRating = rating;
  }

  getAttitudeRating(rating: number) {
    this.customerReview.attitudeRating = rating;
  }

  getCuisineRating(rating: number) {
    this.customerReview.cuisineRating = rating;
  }

  getAtmosphereRating(rating: number) {
    this.customerReview.atmosphereRating = rating;
  }

  reviewCommentValue(event: any) {
    this.customerReview.comment = event.target.value;

  }

  selectRestaurant(restaurant: Restaurant) {
    this.selectedRestaurant = restaurant;
  }
}
