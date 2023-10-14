import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account/account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs';

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
    private formBuilder: FormBuilder) {}

  ngOnInit(): void {
    this.initializeForm();
  }
  initializeForm() {
    this.registerForm = this.formBuilder.group({
      firstName: ['', [Validators.minLength(2), Validators.maxLength(30)]],
      lastName: ['', [Validators.minLength(2), Validators.maxLength(30)]],
      city: ['', []],
      birthDate: ['', []],
      email: ['', [Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
      password: ['', [Validators.minLength(6), Validators.maxLength(30)]]
  })}
  registerEmployee(){
    this.submitted = true;
    this.errorMessages = [];

    this.accountService.registerEmployee(this.registerForm.value).subscribe({
      next: (response) => {
        console.log(response);
      },
      error: error => {
        console.log(error);
      }     
    })
    console.log(this.registerForm.value);
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
