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

  searchLocationPropmt: string = '';
  resultsLocationSearch: string[] = [];
  selectedLocation: string | undefined;

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
      fullLocationString: [],
      longitude: [],
      latitude: [],
      address1: [],
      address2: [],
      city: [],
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

    const sub = this.editRestaurantForm.get('fullLocationString')?.valueChanges.subscribe(loc => {
      if (loc === '') {
        this.editRestaurantForm.get('fullLocationString')?.setErrors({ 'required': true });
      } else {
        this.editRestaurantForm.get('fullLocationString')?.setErrors(null);
      }
    });

    if (this.selectedLocation != this.searchLocationPropmt) {
      this.editRestaurantForm.get('fullLocationString')?.setErrors({ 'required': true });
    }

    if (this.editRestaurantForm.valid && this.restaurant) {
      this.setFormValues();
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

  getSearchLocationResults() {
    fetch(`https://api.geoapify.com/v1/geocode/autocomplete?text=${this.searchLocationPropmt}&apiKey=0d4bc92697134fac82ff67220bd007e2&limit=5`, { method: 'GET' })
      .then(response => response.json())
      .then(result => {
        this.resultsLocationSearch = [];
        for (let res of result.features) {
          const currentResult = res.properties;
          this.resultsLocationSearch.push(currentResult.lon + '|'
            + currentResult.lat + '|'
            + currentResult.address_line1 + '|'
            + currentResult.address_line2 + '|'
            + currentResult.city);
        }
      })
      .catch(error => console.log('error', error));
  }

  selectCity(location: string) {
    this.selectedLocation = location;
    this.searchLocationPropmt = this.selectedLocation;
    document.getElementById('collapseToggle')?.click();
  }

  private setFormValues() {
    const loc = this.selectedLocation?.split('|');
    if (loc) {
      this.editRestaurantForm.get('longitude')?.setValue(loc[0]);
      this.editRestaurantForm.get('latitude')?.setValue(loc[1]);
      this.editRestaurantForm.get('address1')?.setValue(loc[2]);
      this.editRestaurantForm.get('address2')?.setValue(loc[3]);
      this.editRestaurantForm.get('city')?.setValue(loc[4]);
    }
  }
}

