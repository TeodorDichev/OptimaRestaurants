import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { Restaurant } from '../../models/restaurant/restaurant';
import { CustomerReview } from '../../models/reviews/customer-review';
import { ReviewEmployeeInfo } from '../../models/reviews/review-employee-info';
import { AccountService } from '../../pages-routing/account/account.service';
import { ReviewsService } from '../../pages-routing/review/reviews.service';
import { SharedService } from '../../shared.service';
import { User } from '../../models/account/user';

@Component({
  selector: 'app-review',
  templateUrl: './review.component.html',
  styleUrls: ['./review.component.css']
})
export class ReviewsComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  user: User | undefined;

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
    this.getUser();
    this.authorizeRequest();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private getUser() {
    const sub = this.accountService.user$.subscribe({
      next: (response: any) => {
        this.user = response;
      }
    });
    this.subscriptions.push(sub);
  }

  private authorizeRequest() {
    if (this.user) {
      this.sharedService.showNotification(false, 'Неуспешно влизане.', 'Оценки могат да бъдат оставяни само от клиенти!')
      this.router.navigateByUrl('');
    }
    else {
      const sub = this.activatedRoute.queryParamMap.subscribe({
        next: (params: any) => {
          this.getCustomerReviewForm(params.get('email'), params.get('token'));
        }
      });
      this.subscriptions.push(sub);
    }
  }

  getCustomerReviewForm(email: string, token: string) {
    if (token && email) {
      const sub = this.reviewsService.getCustomerReviewForm(email, token).subscribe({
        next: (response: any) => {
          this.reviewEmployeeInfo = response;
        }, error: error => {
          this.router.navigateByUrl('');
          this.sharedService.showNotification(false, 'Невалиден токен!', error.error);
        }
      });
      this.subscriptions.push(sub);
    } else {
      this.router.navigateByUrl('');
      this.sharedService.showNotification(false, 'Липсващ токен!', 'Моля проверете дали сте сканирали правилно QR кода.');
    }
  }

  submitCustomerReview() {
    if (this.reviewEmployeeInfo && this.selectedRestaurant) {
      this.customerReview.restaurantId = this.selectedRestaurant.id;
      this.customerReview.employeeEmail = this.reviewEmployeeInfo.employeeEmail;
    }

    if (this.customerReview.comment.length > 300) {
      this.sharedService.showNotification(false, 'Твърде дълго съобщение!', 'Вашето съобщение трябва да се състои от максимум 300 символа.');
    }
    else if (this.customerReview.atmosphereRating == 0
      && this.customerReview.attitudeRating == 0
      && this.customerReview.atmosphereRating == 0
      && this.customerReview.cuisineRating == 0
    ) {
      this.sharedService.showNotification(false, 'Невалидна оценка!', 'Моля оставете оценка на поне една категория.');
    }
    else {
      const sub = this.reviewsService.submitCustomerReview(this.customerReview).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.router.navigateByUrl('');
        }, error: error => {
          this.sharedService.showNotification(false, 'Неуспешно изпращане!', error.error);
        }
      });
      this.subscriptions.push(sub);
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
