import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
import { EmployeeService } from 'src/app/shared/pages-routing/employee/employee.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-edit-employee',
  templateUrl: './edit-employee.component.html',
  styleUrls: ['./edit-employee.component.css']
})
export class EditEmployeeComponent {
  editEmployeeForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];
  email: string | null = this.accountService.getEmail();

  constructor(public bsModalRef: BsModalRef,
    private formBuilder: FormBuilder,
    private employeeService: EmployeeService,
    private sharedService: SharedService,
    private accountService: AccountService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.editEmployeeForm = this.formBuilder.group({
      newFirstName: ['', []],
      newLastName: ['', []],
      newPhoneNumber: ['', []],
      profilePictureFile: ['', []],
      newBirthDate: ['', []], 
      newCity: ['', []],
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
          this.bsModalRef.hide();
          this.sharedService.showNotification(true, 'Успешно обновен акаунт!', 'Вашият акаунт беше обновен успешно!');
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
      this.employeeService.deleteEmployeeAccount(this.email).subscribe({
        next: (response: any) => {
          this.bsModalRef.hide();
          this.sharedService.showNotification(true, response.value.title, response.value.message);
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
}

