import { Component, OnInit} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Restraurant } from 'src/app/shared/models/restaurant/restaurant';
import { AccountService } from 'src/app/shared/pages/page-routing/account/account.service';
import { ManagerService } from 'src/app/shared/pages/page-routing/manager/manager.service';

@Component({
  selector: 'app-new-restaurant-input-modal',
  templateUrl: './new-restaurant-input-modal.component.html',
  styleUrls: ['./new-restaurant-input-modal.component.css',
  '../../../../../app.component.css']
})
export class NewRestaurantInputModalComponent implements OnInit {

  newRestaurantForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];
  email: string | null = this.accountService.getEmail();

  constructor(public bsModalRef: BsModalRef,
    private formBuilder: FormBuilder,
    private managerService: ManagerService,
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
      iconUrl: ['', []]
    })
  }

  addNewRestaurant() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.newRestaurantForm.valid && this.email) {
      this.managerService.addNewRestaurant(this.newRestaurantForm.value, this.email).subscribe({
        next: (response: any) => {
          this.managerService.setManager(response);
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

  onImageSelected(event: any): void {
    const file = event.target.files[0];

    if (file) {
      this.uploadImage(file);
    }
  }

  uploadImage(file: File): void {
    // Implement the logic to upload the image to our server
    // After the image is uploaded, set the URL to the restaurant.iconUrl property

    // For example, with a service named imageService
    // this.imageService.uploadImage(file).subscribe((imageUrl) => {
    //   this.restaurant.iconUrl = imageUrl;
    // });
  }

  saveImage(restaurant: Restraurant): void {
    // Save the restaurant object with the image URL
    console.log('Image URL:', restaurant.iconUrl);
    // Implement the logic to save the restaurant object
  }

}
