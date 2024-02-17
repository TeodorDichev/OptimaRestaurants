import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { Employee } from 'src/app/shared/models/employee/employee';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
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
  email: string | null = this.accountService.getEmail();
  employee: Employee | undefined;

  constructor(public bsModalRef: BsModalRef,
    private formBuilder: FormBuilder,
    private employeeService: EmployeeService,
    private sharedService: SharedService,
    private accountService: AccountService) { }

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
      isLookingForJob: ['', []]
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

  editEmployee() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.editEmployeeForm.valid && this.email) {
      this.employeeService.updateEmployeeAccount(this.editEmployeeForm.value, this.email).subscribe({
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
    }
  }

  deleteEmployeeAccount() {
    if (this.email) {
      const sub = this.employeeService.deleteEmployeeAccount(this.email).subscribe({
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
}

