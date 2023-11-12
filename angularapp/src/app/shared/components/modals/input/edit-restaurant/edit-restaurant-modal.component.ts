import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Restraurant } from 'src/app/shared/models/restaurant/restaurant';
import { ManagerService } from 'src/app/shared/pages/page-routing/manager/manager.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-edit-restaurant-modal',
  templateUrl: './edit-restaurant-modal.component.html',
  styleUrls: ['./edit-restaurant-modal.component.css',
    '../../../../../app.component.css']
})
export class EditRestaurantModalComponent implements OnInit {
  @Input() currentRestaurant: Restraurant | undefined;

  editRestaurantForm: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];

  constructor(public bsModalRef: BsModalRef,
    private formBuilder: FormBuilder,
    private sharedService: SharedService,
    private managerService: ManagerService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.editRestaurantForm = this.formBuilder.group({
      name: ['', []],
      address: ['', []],
      city: ['', []],
      employeeCapacity: ['', []],
      iconUrl: ['', []]
    })
  }

  editRestaurant() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.editRestaurantForm.valid && this.currentRestaurant) {
      this.managerService.editRestaurant(this.editRestaurantForm.value, this.currentRestaurant.id).subscribe({
        next: (response: any) => {
          console.log(response);
          this.currentRestaurant = this.editRestaurantForm.value;
          this.bsModalRef.hide();
          this.sharedService.showNotification(true, response.value.title, response.value.message);
        },
        error: error => {
          console.log(error.error);
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

