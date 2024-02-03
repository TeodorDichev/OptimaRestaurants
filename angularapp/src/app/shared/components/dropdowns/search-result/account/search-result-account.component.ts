import { SharedService } from 'src/app/shared/shared.service';
import { Component, Input } from '@angular/core';
import { SearchResultAccount } from 'src/app/shared/models/account/search-result-account';

@Component({
  selector: 'app-search-result-account',
  templateUrl: './search-result-account.component.html',
  styleUrls: ['./search-result-account.component.css']
})
export class SearchResultAccountComponent {
  @Input() searchResult: SearchResultAccount[] | undefined;

  constructor(private sharedService: SharedService) { }

  openInfoModal(result: SearchResultAccount) {
    this.sharedService.openUserInfoModal(result.email, result.role);
  }
}
