import { Component, OnDestroy, OnInit } from '@angular/core';
import { AccountService } from '../../pages-routing/account/account.service';
import { SharedService } from '../../shared.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmEmail } from '../../models/account/confirm-email';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];
  success: boolean = true;

  constructor(private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    const sub = this.activatedRoute.queryParamMap.subscribe({
      next: (params: any) => {
        const confirmEmail: ConfirmEmail = {
          token: params.get('token'),
          email: params.get('email')
        }
        this.accountService.confirmEmail(confirmEmail).subscribe({
          next: (response: any) => {
            this.sharedService.showNotification(true, response.value.title, response.value.message);
          },
          error: error => {
            this.success = false;
            this.sharedService.showNotification(false, "Неуспешно потвърждаване на акаунта", error.error);
          }
        })
      }
    })
    this.subscriptions.push(sub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  resendEmailConfirmaitonLink() {
    this.router.navigateByUrl('/account/send-email/resend-email-confirmation-link');
  }
}
