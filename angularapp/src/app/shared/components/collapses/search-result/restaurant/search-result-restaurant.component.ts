import { Component, Input } from '@angular/core';
import { SearchResultAccount } from 'src/app/shared/models/account/search-result-account';
import { SearchResultRestaurant } from 'src/app/shared/models/restaurant/search-result-restaurant';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-search-result-restaurant',
  templateUrl: './search-result-restaurant.component.html',
  styleUrls: ['./search-result-restaurant.component.css']
})
export class SearchResultRestaurantComponent {
  @Input() searchResult: SearchResultRestaurant[] | undefined;

  constructor(private sharedService: SharedService) { }

  openInfoModal(result: SearchResultAccount) {
    
  }
}
