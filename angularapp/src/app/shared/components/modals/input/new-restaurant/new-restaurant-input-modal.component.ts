import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-new-restaurant-input-modal',
  templateUrl: './new-restaurant-input-modal.component.html',
  styleUrls: ['./new-restaurant-input-modal.component.css']
})
export class NewRestaurantInputModalComponent implements OnInit {

  newRestaurantForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];
  email: string | null = this.accountService.getEmail();

  constructor(public bsModalRef: BsModalRef,
    private formBuilder: FormBuilder,
    private managerService: ManagerService,
    private sharedService: SharedService,
    private accountService: AccountService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.newRestaurantForm = this.formBuilder.group({
      name: ['', [Validators.required]],
      address: ['', [Validators.required]],
      city: ['', [Validators.required]],
      employeeCapacity: ['', [Validators.required]],
      iconFile: ['', []]
    })
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
        this.newRestaurantForm.patchValue({
            iconFile: file
        });
    }
  }

  addNewRestaurant() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.newRestaurantForm.valid && this.email) {
      this.managerService.addNewRestaurant(this.newRestaurantForm.value, this.email).subscribe({
        next: (response: any) => {
          this.managerService.setManager(response);
          this.bsModalRef.hide();
          this.sharedService.showNotification(true, 'Успешно създаден ресторант!', 'Вашият ресторант беше създаден успешно!');
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
