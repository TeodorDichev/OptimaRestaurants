import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReviewsComponent } from '../../restaurants/reviews/reviews.component';

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
