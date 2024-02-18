import { Component, Input, OnDestroy } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { ManagerReview } from 'src/app/shared/models/reviews/manager-review';
import { ReviewsService } from 'src/app/shared/pages-routing/review/reviews.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-manager-review',
  templateUrl: './manager-review.component.html',
  styleUrls: ['./manager-review.component.css']
})
export class ManagerReviewComponent implements OnDestroy {
  private subscriptions: Subscription[] = [];

  @Input() employeeEmail!: string;
  @Input() restaurantId!: string;

  managerReview: ManagerReview = {
    employeeEmail: '',
    restaurantId: '',
    comment: '',
    punctualityRating: 0,
    collegialityRating: 0
  };

  constructor(private sharedService: SharedService,
    private reviewsService: ReviewsService,
    private bsModalRef: BsModalRef) { };

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  submitManagerReview() {
    this.managerReview.employeeEmail = this.employeeEmail;
    this.managerReview.restaurantId = this.restaurantId;

    if (this.managerReview.comment.length > 300) {
      this.sharedService.showNotification(false, 'Твърде дълго съобщение!', 'Вашето съобщение трябва да се състои от максимум 300 символа.');
    }
    else if (this.managerReview.collegialityRating == 0
      && this.managerReview.punctualityRating == 0
    ) {
      this.sharedService.showNotification(false, 'Невалидна оценка!', 'Моля оставете оценка на поне една категория.');
    }
    else {
      const sub = this.reviewsService.submitManagerReview(this.managerReview).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.bsModalRef.hide();
        }, error: error => {
          console.log(this.managerReview);
          this.sharedService.showNotification(false, 'Неуспешно изпращане!', error.error);
        }
      });
      this.subscriptions.push(sub);
    }
  }

  getCollegialityRating(rating: number) {
    this.managerReview.collegialityRating = rating;
  }

  getPunctualityRating(rating: number) {
    this.managerReview.punctualityRating = rating;
  }

  getComment(event: any) {
    this.managerReview.comment = event.target.value;
  }
}
