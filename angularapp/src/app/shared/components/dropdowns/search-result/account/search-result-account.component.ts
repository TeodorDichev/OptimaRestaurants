import { SharedService } from 'src/app/shared/shared.service';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { SearchResultAccount } from 'src/app/shared/models/account/search-result-account';
import { User } from 'src/app/shared/models/account/user';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-search-result-account',
  templateUrl: './search-result-account.component.html',
  styleUrls: ['./search-result-account.component.css']
})
export class SearchResultAccountComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = []
  @Input() searchResult: SearchResultAccount[] | undefined;

  user: User | undefined;

  constructor(private sharedService: SharedService,
    private accountService: AccountService) { }

  ngOnInit(): void {
    this.getUser();
  }
  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private getUser() {
    const sub = this.accountService.user$.subscribe({
      next: (response: any) => {
        this.user = response;
      }
    });
    this.subscriptions.push(sub);
  }

  openInfoModal(result: SearchResultAccount) {
    this.sharedService.openUserInfoModal(result.email, result.role);
  }

  missingIcon(result: SearchResultAccount) {
    //result.picturePath = 'assets/images/logo-bw-with-bg.png';
  }
}
