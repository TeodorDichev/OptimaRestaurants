import { Component, Input } from '@angular/core';
import { Restaurant } from 'src/app/shared/models/restaurant/restaurant';

@Component({
  selector: 'app-restaurant-info',
  templateUrl: './restaurant-info.component.html',
  styleUrls: ['./restaurant-info.component.css']
})
export class RestaurantInfoComponent {
  @Input() restaurant: Restaurant | undefined;



}
