import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ReviewsService } from '../../pages-routing/review/reviews.service';
import { SharedService } from '../../shared.service';

@Component({
  selector: 'app-reviews',
  templateUrl: './reviews.component.html',
  styleUrls: ['./reviews.component.css']
})
export class ReviewsComponent implements OnInit {

  token: string | undefined;
  email: string | undefined;
  errorMessages: string[] = [];
  constructor(private reviewsService: ReviewsService,
    private sharedService: SharedService,
    private router: Router,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.authorizeRequest();
  }

  private authorizeRequest() {
    this.activatedRoute.queryParamMap.subscribe({
      next: (params: any) => {
        this.token = params.get('token');
        this.email = params.get('email');
        if (this.token && this.email) {
          this.reviewsService.getCustomerReviewForm(this.email, this.token).subscribe({
            next: () => { }, error: error => {
              this.router.navigateByUrl('');
              this.sharedService.showNotification(false, 'Невалиден токен!', error.error);
            }
          });
        } else {
          this.router.navigateByUrl('');
          this.sharedService.showNotification(false, 'Липсващ токен!', 'Моля проверете дали сте сканирали правилно QR кода.');
        }
      }
    })
  }

  getCustomerReviewForm() {

  }

  submitCustomerReview() {

  }

  submitManagerReview() {

  }


}
