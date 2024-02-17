import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-restaurant-map',
  templateUrl: './restaurant-map.component.html',
  styleUrls: ['./restaurant-map.component.css']
})
export class RestaurantMapComponent {
  @Input() longitude!: number;
  @Input() latitude!: number;

  apiKey: string = 'AIzaSyDeehPso1ckCJYdYSHJ12b3buopi8tEJAM';
}
