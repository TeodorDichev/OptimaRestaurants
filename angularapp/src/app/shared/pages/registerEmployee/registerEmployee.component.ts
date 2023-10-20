import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account-routing/account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedService } from '../../shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registerEmployee',
  templateUrl: './registerEmployee.component.html',
  styleUrls: ['./registerEmployee.component.css']
})
export class RegisterEmployeeComponent implements OnInit{
  registerForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];

  constructor(private accountService: AccountService, 
    private sharedService: SharedService,
    private router: Router,
    private formBuilder: FormBuilder) {}

  ngOnInit(): void {
    this.initializeForm();
  }
  initializeForm() {
    this.registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]],
      lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]],
      city: ['', [Validators.required]],
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
        this.sharedService.showNotification(true, response.value.title, response.value.message);
        this.router.navigateByUrl('/account/next');
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
