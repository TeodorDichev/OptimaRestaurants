import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { User } from '../../models/account/user';
import { AccountService } from '../../pages-routing/account/account.service';
import { EmployeeService } from '../../pages-routing/employee/employee.service';
import { ManagerService } from '../../pages-routing/manager/manager.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  user: User | undefined;

  loginForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];

  constructor(private accountService: AccountService,
    private managerService: ManagerService,
    private employeeService: EmployeeService,
    private router: Router,
    private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  initializeForm() {
    this.loginForm = this.formBuilder.group({
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]]
    })
  }

  login() {
    this.submitted = true;
    this.errorMessages = [];
    if (this.loginForm.valid) {
      const sub = this.accountService.login(this.loginForm.value).subscribe({
        next: (response: any) => {
          this.user = response;
          this.setUserRole();
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

  private setUserRole() {
    if (this.user) {
      if (this.user.isManager) {
        this.managerService.getManager(this.user.email).subscribe({
          next: (response: any) => {
            this.managerService.setManager(response);
          }
        });
      }
      else {
        this.employeeService.getEmployee(this.user.email).subscribe({
          next: (response: any) => {
            this.employeeService.setEmployee(response);
          }
        });
      }
    }
  }

  resendEmailConfirmaitonLink() {
    this.router.navigateByUrl('/account/send-email/resend-email-confirmation-link');
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
