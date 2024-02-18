import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { Manager } from 'src/app/shared/models/manager/manager';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-edit-manager',
  templateUrl: './edit-manager.component.html',
  styleUrls: ['./edit-manager.component.css']
})
export class EditManagerComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

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

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  initializeForm() {
    this.editManagerForm = this.formBuilder.group({
      newFirstName: ['', [Validators.minLength(2), Validators.maxLength(50)]],
      newLastName: ['', [Validators.minLength(2), Validators.maxLength(50)]],
      newPhoneNumber: ['', [Validators.pattern('^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$')]],
      profilePictureFile: ['', []],
      oldPassword: ['', []],
      newPassword: ['', []]
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
      const sub = this.managerService.updateManagerAccount(this.editManagerForm.value, this.email).subscribe({
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
      this.subscriptions.push(sub);
    }
  }

  deleteManagerAccount() {
    if (this.email) {
      const sub = this.managerService.deleteManagerAccount(this.email).subscribe({
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

  private setManager() {
    const sub = this.managerService.manager$.subscribe({
      next: (response: any) => {
        this.manager = response;
      }
    });
    this.subscriptions.push(sub);
  }
}
