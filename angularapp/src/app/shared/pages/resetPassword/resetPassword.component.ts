import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account-routing/account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedService } from '../../shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { take } from 'rxjs';
import { User } from '../../models/account/user';

@Component({
  selector: 'app-resetPassword',
  templateUrl: './resetPassword.component.html',
  styleUrls: ['./resetPassword.component.css']
})
export class ResetPasswordComponent implements OnInit {
 resetPasswordForm: FormGroup = new FormGroup({});
 token: string | undefined;
 email: string | undefined;
 submitted = false;
 errorMessages: string[] = [];

 constructor(private accountService: AccountService,
  private sharedService: SharedService,
  private formBuilder: FormBuilder,
  private router: Router,
  private activatedRoute: ActivatedRoute) {}
  ngOnInit(): void {
    this.accountService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user) {
          this.router.navigateByUrl('/');
        } else {
          this.activatedRoute.queryParamMap.subscribe({
            next: (params: any) => {
              this.token = params.get('toker');
              this.email = params.get('email');
              if (this.token && this.email){
                this.initializeForm(this.email);
              }
            }
          })
        }
      }
    })
  }

  initializeForm(username: string){
    this.resetPasswordForm = this.formBuilder.group({
      email: [{value: username, disabled: true}],
      newPassword: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(30)]]
    })
  }

  resetPassword(){

  }

}
