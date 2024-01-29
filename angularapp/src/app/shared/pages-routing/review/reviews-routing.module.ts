import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReviewsComponent } from '../../user-pages/reviews/review-customer.component';

const routes: Routes = [
  {path: '', component: ReviewsComponent}
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ], 
  exports: [
    RouterModule
  ]
})
export class ReviewsRoutingModule { }
