import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AccountService } from '../../pages-routing/account/account.service';
import { EmployeeService } from '../../pages-routing/employee/employee.service';
import { SharedService } from '../../shared.service';

@Component({
  selector: 'app-register-employee',
  templateUrl: './register-employee.component.html',
  styleUrls: ['./register-employee.component.css']
})
export class RegisterEmployeeComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  searchLocationPropmt: string = '';
  resultsLocationSearch: string[] = [];
  selectedCity: string | undefined;

  registerForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];

  constructor(private accountService: AccountService,
    private sharedService: SharedService,
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
    this.registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      city: ['', [Validators.required]],
      birthDate: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(50)]]
    });
  }

  registerEmployee() {
    this.submitted = true;
    this.errorMessages = [];

    const sub = this.registerForm.get('city')?.valueChanges.subscribe(city => {
      if (city === '') {
        this.registerForm.get('city')?.setErrors({ 'required': true });
      } else {
        this.registerForm.get('city')?.setErrors(null);
      }
    });
    
    if (this.selectedCity != this.searchLocationPropmt) {
      this.registerForm.get('city')?.setErrors({ 'required': true });
    }
    
    if (sub) this.subscriptions.push(sub);

    if (this.registerForm.valid) {
      const sub = this.accountService.registerEmployee(this.registerForm.value).subscribe({
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
          if (error.error.errors) {
            this.errorMessages = error.error.errors;
          } else {
            this.errorMessages.push(error.error);
          }
        }
      });
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

  getSearchLocationResults() {
    fetch(`https://api.geoapify.com/v1/geocode/autocomplete?text=${this.searchLocationPropmt}&apiKey=0d4bc92697134fac82ff67220bd007e2&limit=3`, { method: 'GET' })
      .then(response => response.json())
      .then(result => {
        this.resultsLocationSearch = [];
        for (let res of result.features) {
          this.resultsLocationSearch.push(res.properties.city + ', ' + res.properties.country);
        }
        console.log(this.resultsLocationSearch);
      })
      .catch(error => console.log('error', error));
  }

  selectCity(cityCountry: string) {
    this.selectedCity = cityCountry.split(',')[0];
    this.searchLocationPropmt = this.selectedCity;
    document.getElementById('collapseToggle')?.click();
  }
}
