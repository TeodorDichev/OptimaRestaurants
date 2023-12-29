import { SharedService } from 'src/app/shared/shared.service';
import { Component, Input } from '@angular/core';
import { SearchResult } from 'src/app/shared/models/account/search-result';

@Component({
  selector: 'app-search-result',
  templateUrl: './search-result.component.html',
  styleUrls: ['./search-result.component.css']
})
export class SearchResultComponent {
  @Input() searchResult: SearchResult[] | undefined;

  constructor(private sharedService: SharedService) {}

  openInfoModal(result: SearchResult) {
    this.sharedService.openUserInfoModal(result.email, result.role);
  }
}
