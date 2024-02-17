import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AccountService } from '../../pages-routing/account/account.service';
import { ManagerService } from '../../pages-routing/manager/manager.service';
import { SharedService } from '../../shared.service';

@Component({
  selector: 'app-register-manager',
  templateUrl: './register-manager.component.html',
  styleUrls: ['./register-manager.component.css']
})
export class RegisterManagerComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  registerForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];

  constructor(private accountService: AccountService,
    private sharedService: SharedService,
    private managerService: ManagerService,
    private router: Router,
    private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  initializeForm() {
    this.registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(50)]]
    })
  }

  registerManager() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.registerForm.valid) {
      const sub = this.accountService.registerManager(this.registerForm.value).subscribe({
        next: (response: any) => {
          this.accountService.setUser(response);
          const sub1 = this.managerService.getManager(response.email).subscribe({
            next: (resp: any) => {
              this.managerService.setManager(resp);
            }
          })
          this.subscriptions.push(sub1);
          this.sharedService.showNotification(true, 'Успешно създаден акаунт!', 'Вашият акаунт беше успешно създаден! Моля, потвърдете имейл адреса си.');
          this.router.navigateByUrl('/manager');
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

  isText: boolean = false;
  type: string = "Password";
  eyeIcon: string = "fa-eye-slash";

  hideShowPass() {
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }
}
