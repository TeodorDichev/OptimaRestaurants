import { Component, Input, OnInit } from '@angular/core';
import { Restaurant } from 'src/app/shared/models/restaurant/restaurant';

@Component({
  selector: 'app-restaurant-info',
  templateUrl: './restaurant-info.component.html',
  styleUrls: ['./restaurant-info.component.css']
})
export class RestaurantInfoComponent implements OnInit{
  @Input() restaurant: Restaurant | undefined;

  ngOnInit() {

  }
}
