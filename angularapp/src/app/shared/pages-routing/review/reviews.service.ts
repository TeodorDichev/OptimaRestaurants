import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { CustomerReview } from '../../models/reviews/customer-review';

@Injectable({
  providedIn: 'root'
})
export class ReviewsService {

  constructor(private http: HttpClient) { }

  getCustomerReviewForm(email: string, token: string) {
    return this.http.get(`${environment.appUrl}/api/review-employee/${email}/${token}`);
  }

  submitCustomerReview(customerReview: CustomerReview) {
    return this.http.post(`${environment.appUrl}/api/review-employee`, customerReview);
  }

  submitManagerReview() {
    return this.http.get(`${environment.appUrl}/api/manager/review-employee`);
  }

  getEmployeeReviewsHistory(email:string) {
    return this.http.get(`${environment.appUrl}/api/get-reviews-history/${email}`);
  }
}
