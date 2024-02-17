import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../../pages-routing/account/account.service';
import { EmployeeService } from '../../pages-routing/employee/employee.service';
import { ManagerService } from '../../pages-routing/manager/manager.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

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
    //this.subscriptions.forEach(subscription => subscription.unsubscribe());
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
          this.accountService.setUser(response);
          if (response.isManager == false) {
            const sub1 = this.employeeService.getEmployee(response.email).subscribe({
              next: (resp: any) => {
                this.employeeService.setEmployee(resp);
              }
            })
            this.subscriptions.push(sub1);
          }
          else if (response.isManager == true) {
            const sub2 = this.managerService.getManager(response.email).subscribe({
              next: (resp: any) => {
                this.managerService.setManager(resp);
              }
            })
            this.subscriptions.push(sub2);
          }
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
