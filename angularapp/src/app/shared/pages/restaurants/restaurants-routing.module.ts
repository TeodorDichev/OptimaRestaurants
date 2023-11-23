import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BrowseAllRestaurantsComponent } from '../../restaurants/browse-all-restaurants/browse-all-restaurants.component';

const routes: Routes = [
  {path: '', component: BrowseAllRestaurantsComponent}
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
export class RestaurantsRoutingModule { }
