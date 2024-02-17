import { Component, OnDestroy, OnInit } from '@angular/core';
import { AccountService } from '../../pages-routing/account/account.service';
import { SharedService } from '../../shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, take } from 'rxjs';
import { User } from '../../models/account/user';

@Component({
  selector: 'app-send-email',
  templateUrl: './send-email.component.html',
  styleUrls: ['./send-email.component.css']
})
export class SendEmailComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  title: string = '';
  emailForm: FormGroup = new FormGroup({});
  submitted = false;
  mode: string | undefined;
  errorMessages: string[] = [];

  constructor(private accountService: AccountService,
    private sharedService: SharedService,
    private formBuilder: FormBuilder,
    private router: Router,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    const sub = this.accountService.user$.subscribe({
      next: (user: User | null) => {
        if (user) {
          this.router.navigateByUrl('/');
        } else {
          const mode = this.activatedRoute.snapshot.paramMap.get('mode');
          if (mode) {
            this.mode = mode;
            if (this.mode.includes('resend-email-confirmation-link'))
              this.title = 'Повторно изпращане на имейл за потвърждаване';
            else
              this.title = 'Нулиране на забравена парола';
            this.initializeForm();
          }
        }
      }
    })
    this.subscriptions.push(sub);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  initializeForm() {
    this.emailForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]]
    })
  }

  sendEmail() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.emailForm.valid && this.mode) {
      if (this.mode.includes('resend-email-confirmation-link')) {
        const sub = this.accountService.resendEmailConfirmationLink(this.emailForm.get('email')?.value).subscribe({
          next: (response: any) => {
            this.sharedService.showNotification(true, response.value.title, response.value.message);
            this.router.navigateByUrl('/account/login');
          },
          error: error => {
            if (error.error.errors) {
              this.errorMessages = error.error.errors;
            } else {
              this.errorMessages.push(error.error);
            }
          }
        })
        this.subscriptions.push(sub);
      } else if (this.mode.includes('forgot-username-or-password')) {
        const sub = this.accountService.resetPassword(this.emailForm.get('email')?.value).subscribe({
          next: (response: any) => {
            this.sharedService.showNotification(true, response.value.title, response.value.message);
            this.router.navigateByUrl('/account/login');
          },
          error: error => {
            if (error.error.errors) {
              this.errorMessages = error.error.errors;
            } else {
              this.errorMessages.push(error.error);
            }
          }
        })
        this.subscriptions.push(sub);
      }
    }

  }

  cancel() {
    this.router.navigateByUrl('/account/login');
  }
}
