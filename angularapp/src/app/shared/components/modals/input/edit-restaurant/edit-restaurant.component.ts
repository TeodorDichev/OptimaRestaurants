import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { Restaurant } from 'src/app/shared/models/restaurant/restaurant';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-edit-restaurant',
  templateUrl: './edit-restaurant.component.html',
  styleUrls: ['./edit-restaurant.component.css']
})
export class EditRestaurantModalComponent implements OnInit {
  @Input() restaurant: Restaurant | undefined;
  private subscriptions: Subscription[] = [];

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

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  initializeForm() {
    this.editRestaurantForm = this.formBuilder.group({
      name: ['', [Validators.minLength(2), Validators.maxLength(50)]],
      address: ['', [Validators.minLength(2), Validators.maxLength(50)]],
      city: ['', [Validators.minLength(2)]],
      employeeCapacity: ['', [Validators.pattern('[0-9]+')]],
      iconFile: ['', []],
      isWorking: ['', []]
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
      const sub = this.managerService.editRestaurant(this.editRestaurantForm.value, this.restaurant.id).subscribe({
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
      this.subscriptions.push(sub);
    }
  }

  deleteRestaurant() {
    if (this.restaurant) {
      const sub = this.managerService.deleteRestaurant(this.restaurant.id).subscribe({
        next: (response: any) => {
          this.sharedService.showNotification(true, "Успешно премахнат ресторант!", "Вашият ресторант беше успешно изтрит.");
          this.bsModalRef.hide();
          this.managerService.setManager(response);
        }
      });
      this.subscriptions.push(sub);
    }
  }
}

