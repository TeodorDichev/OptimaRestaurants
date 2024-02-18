import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { Employee } from 'src/app/shared/models/employee/employee';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-edit-employee',
  templateUrl: './edit-employee.component.html',
  styleUrls: ['./edit-employee.component.css']
})
export class EditEmployeeComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  editEmployeeForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];
  employee: Employee | undefined;

  searchLocationPropmt: string | undefined;
  resultsLocationSearch: string[] = [];
  selectedCity: string | undefined;

  constructor(public bsModalRef: BsModalRef,
    private formBuilder: FormBuilder,
    private employeeService: EmployeeService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getEmployee();
    this.initializeForm();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  initializeForm() {
    this.editEmployeeForm = this.formBuilder.group({
      newFirstName: ['', [Validators.minLength(2), Validators.maxLength(50)]],
      newLastName: ['', [Validators.minLength(2), Validators.maxLength(50)]],
      newPhoneNumber: ['', [Validators.pattern('^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$')]],
      profilePictureFile: ['', []],
      newBirthDate: ['', []],
      newCity: ['', [Validators.minLength(2)]],
      isLookingForJob: ['', []],
      oldPassword: ['', []],
      newPassword: ['', []]
    })
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      this.editEmployeeForm.patchValue({
        profilePictureFile: file
      });
    }
  }

  getSearchLocationResults() {
    fetch(`https://api.geoapify.com/v1/geocode/autocomplete?text=${this.searchLocationPropmt}&apiKey=0d4bc92697134fac82ff67220bd007e2&limit=3`, { method: 'GET' })
      .then(response => response.json())
      .then(result => {
        this.resultsLocationSearch = [];
        for (let res of result.features) {
          this.resultsLocationSearch.push(res.properties.city + ', ' + res.properties.country);
        }
      })
      .catch(error => console.log('error', error));
  }

  selectCity(cityCountry: string) {
    this.selectedCity = cityCountry.split(',')[0];
    this.searchLocationPropmt = this.selectedCity;
    document.getElementById('collapseToggle')?.click();
  }

  editEmployee() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.selectedCity && this.selectedCity != this.searchLocationPropmt) {
      this.editEmployeeForm.get('newCity')?.setErrors({ 'invalid': true });
    }

    if (this.editEmployeeForm.valid && this.employee) {
      const sub = this.employeeService.updateEmployeeAccount(this.editEmployeeForm.value, this.employee.email).subscribe({
        next: (response: any) => {
          this.employeeService.setEmployee(response);
          this.sharedService.showNotification(true, 'Успешно обновен акаунт!', 'Вашият акаунт беше обновен успешно!');
          this.bsModalRef.hide();
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

  deleteEmployeeAccount() {
    if (this.employee) {
      const sub = this.employeeService.deleteEmployeeAccount(this.employee.email).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, response.value.title, response.value.message);
          this.bsModalRef.hide();
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

  private getEmployee() {
    const sub = this.employeeService.employee$.subscribe({
      next: (response: any) => {
        this.employee = response;
      }
    })
    this.subscriptions.push(sub);
  }

  isTextOld: boolean = false;
  typeOld: string = "Password";
  eyeIconOld: string = "fa-eye-slash";

  hideShowPassOld() {
    this.isTextOld = !this.isTextOld;
    this.isTextOld ? this.eyeIconOld = "fa-eye" : this.eyeIconOld = "fa-eye-slash";
    this.isTextOld ? this.typeOld = "text" : this.typeOld = "password";
  }

  isTextNew: boolean = false;
  typeNew: string = "Password";
  eyeIconNew: string = "fa-eye-slash";

  hideShowPassNew() {
    this.isTextNew = !this.isTextNew;
    this.isTextNew ? this.eyeIconNew = "fa-eye" : this.eyeIconNew = "fa-eye-slash";
    this.isTextNew ? this.typeNew = "text" : this.typeNew = "password";
  }
}

