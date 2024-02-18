import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { AccountService } from 'src/app/shared/pages-routing/account/account.service';
import { ManagerService } from 'src/app/shared/pages-routing/manager/manager.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-new-restaurant-input-modal',
  templateUrl: './new-restaurant-input-modal.component.html',
  styleUrls: ['./new-restaurant-input-modal.component.css']
})
export class NewRestaurantInputModalComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  searchLocationPropmt: string = '';
  resultsLocationSearch: string[] = [];
  selectedLocation: string | undefined;

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

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  initializeForm() {
    this.newRestaurantForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      fullLocationString: [],
      longitude: [],
      latitude: [],
      address1: [],
      address2: [],
      city: [],
      employeeCapacity: ['', [Validators.required, Validators.pattern('[0-9]+')]],
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

    const sub = this.newRestaurantForm.get('fullLocationString')?.valueChanges.subscribe(loc => {
      if (loc === '') {
        this.newRestaurantForm.get('fullLocationString')?.setErrors({ 'required': true });
      } else {
        this.newRestaurantForm.get('fullLocationString')?.setErrors(null);
      }
    });

    if (this.selectedLocation != this.searchLocationPropmt) {
      this.newRestaurantForm.get('fullLocationString')?.setErrors({ 'required': true });
    }

    if (sub) this.subscriptions.push(sub);

    if (this.newRestaurantForm.valid && this.email) {
      this.setFormValues();
      const sub = this.managerService.addNewRestaurant(this.newRestaurantForm.value, this.email).subscribe({
        next: (response: any) => {
          this.managerService.setManager(response);
          this.sharedService.showNotification(true, 'Успешно създаден ресторант!', 'Вашият ресторант беше създаден успешно!');
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

  getSearchLocationResults() {
    fetch(`https://api.geoapify.com/v1/geocode/autocomplete?text=${this.searchLocationPropmt}&apiKey=0d4bc92697134fac82ff67220bd007e2&limit=5`, { method: 'GET' })
      .then(response => response.json())
      .then(result => {
        this.resultsLocationSearch = [];
        for (let res of result.features) {
          const currentResult = res.properties;
          console.log(currentResult);
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
      this.newRestaurantForm.get('longitude')?.setValue(loc[0]);
      this.newRestaurantForm.get('latitude')?.setValue(loc[1]);
      this.newRestaurantForm.get('address1')?.setValue(loc[2]);
      this.newRestaurantForm.get('address2')?.setValue(loc[3]);
      this.newRestaurantForm.get('city')?.setValue(loc[4]);
    }
  }
}
