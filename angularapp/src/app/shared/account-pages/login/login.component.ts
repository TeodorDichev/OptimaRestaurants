import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../../pages-routing/account/account.service';
import { EmployeeService } from '../../pages-routing/employee/employee.service';
import { ManagerService } from '../../pages-routing/manager/manager.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
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
      this.accountService.login(this.loginForm.value).subscribe({
        next: (response: any) => {
          this.accountService.setUser(response);
          if (!response.isManager) {
            this.employeeService.getEmployee(response.email).subscribe({
              next: (resp: any) => {
                this.employeeService.setEmployee(resp);
              }
            })
          }
          else if (response.isManager) {
            this.managerService.getManager(response.email).subscribe({
              next: (resp: any) => {
                this.managerService.setManager(resp);
              }
            })
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
