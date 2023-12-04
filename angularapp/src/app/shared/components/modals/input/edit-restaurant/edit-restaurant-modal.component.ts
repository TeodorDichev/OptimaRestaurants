import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Restaurant } from 'src/app/shared/models/restaurant/restaurant';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-edit-restaurant-modal',
  templateUrl: './edit-restaurant-modal.component.html',
  styleUrls: ['./edit-restaurant-modal.component.css']
})
export class EditRestaurantModalComponent implements OnInit {
  @Input() restaurant: Restaurant | undefined;

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
      iconFile: ['', []]
    })
  }
  
  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
        this.editRestaurantForm.patchValue({
          iconFile: file
        });
    }
  }

  editRestaurant() {
    this.submitted = true;
    this.errorMessages = [];
    if (this.editRestaurantForm.valid && this.restaurant) {
      this.managerService.editRestaurant(this.editRestaurantForm.value, this.restaurant.id).subscribe({
        next: (response: any) => {
          this.managerService.setManager(response);
          this.sharedService.showNotification(true, "Успешно обновен ресторант!", "Вашият ресторант беше успешно обновен.");
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
}

