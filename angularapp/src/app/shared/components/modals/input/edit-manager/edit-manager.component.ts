import { Component } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Manager } from 'src/app/shared/models/manager/manager';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-edit-manager',
  templateUrl: './edit-manager.component.html',
  styleUrls: ['./edit-manager.component.css']
})
export class EditManagerComponent {
  editManagerForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];
  email: string | null = this.accountService.getEmail();
  manager: Manager | undefined;

  constructor(public bsModalRef: BsModalRef,
    private formBuilder: FormBuilder,
    private managerService: ManagerService,
    private sharedService: SharedService,
    private accountService: AccountService) { }

  ngOnInit(): void {
    this.setManager();
    this.initializeForm();
  }

  initializeForm() {
    this.editManagerForm = this.formBuilder.group({
      newFirstName: ['', []],
      newLastName: ['', []],
      newPhoneNumber: ['', []],
      profilePictureFile: ['', []]
    })
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
        this.editManagerForm.patchValue({
            profilePictureFile: file
        });
    }
  }

  editManager() {
    this.submitted = true;
    this.errorMessages = [];
    
    if (this.editManagerForm.valid && this.email) {
      this.managerService.updateManagerAccount(this.editManagerForm.value, this.email).subscribe({
        next: (response: any) => {
          this.managerService.setManager(response);
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

  deleteManagerAccount() {
    if (this.email) {
      this.managerService.deleteManagerAccount(this.email).subscribe({
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

  private setManager() {
    this.managerService.manager$.subscribe({
      next: (response: any) => {
        this.manager = response;
      }
    })
  }
}
