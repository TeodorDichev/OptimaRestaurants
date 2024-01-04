import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../pages-routing/account/account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedService } from '../../shared.service';
import { Router } from '@angular/router';
import { EmployeeService } from '../../pages-routing/employee/employee.service';

@Component({
  selector: 'app-register-employee',
  templateUrl: './register-employee.component.html',
  styleUrls: ['./register-employee.component.css']
})
export class RegisterEmployeeComponent implements OnInit{
  registerForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];

  constructor(private accountService: AccountService, 
    private sharedService: SharedService,
    private employeeService: EmployeeService,
    private router: Router,
    private formBuilder: FormBuilder) {}

  ngOnInit(): void {
    this.initializeForm();
  }
  initializeForm() {
    this.registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]],
      lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]],
      city: ['', [Validators.required, Validators.minLength(2)]],
      birthDate: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(30)]]
  })}
  registerEmployee(){
    this.submitted = true;
    this.errorMessages = [];

    if(this.registerForm.valid){
    this.accountService.registerEmployee(this.registerForm.value).subscribe({
      next: (response: any) => {
        this.accountService.setUser(response);
        this.employeeService.getEmployee(response.email).subscribe({
          next: (resp: any) => {
            this.employeeService.setEmployee(resp);
          }
        })
        this.sharedService.showNotification(true, 'Успешно създаден акаунт!', 'Вашият акаунт беше успешно създаден! Моля, потвърдете имейл адреса си.');
        this.router.navigateByUrl('/employee');
      },
      error: error => {
        if(error.error.errors){
          this.errorMessages = error.error.errors;
        } else {
          this.errorMessages.push(error.error);
        }
      }     
    })}
  }
  isText: boolean = false;
  type: string = "Password";
  eyeIcon: string = "fa-eye-slash";
  hideShowPass(){
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }
}
