import { Component, ElementRef, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AccountService } from '../../pages-routing/account/account.service';
import { ManagerService } from '../../pages-routing/manager/manager.service';
import { User } from '../../models/account/user';
import { Router } from '@angular/router';
import { SharedService } from '../../shared.service';
import { EmployeeService } from '../../pages-routing/employee/employee.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SearchResultAccount } from '../../models/account/search-result-account';
import { RestaurantsService } from '../../pages-routing/restaurants/restaurants.service';
import { SearchResultRestaurant } from '../../models/restaurant/search-result-restaurant';
import { Subscription } from 'rxjs';

@Component({
  selector: 'nav-top',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  searchForm: FormGroup = new FormGroup({});
  submitted = false;
  user: User | null | undefined;

  searchResultAccount: SearchResultAccount[] = [];
  searchResultRestaurant: SearchResultRestaurant[] = [];
  @ViewChild('dropdown') dropdown: ElementRef | undefined;
  newNotifications: boolean | undefined;
  searchForAccounts: boolean = true;
  searchingFor: string = 'потребител';

  constructor(private accountService: AccountService,
    private managerService: ManagerService,
    private employeeService: EmployeeService,
    private sharedService: SharedService,
    private restaurantService: RestaurantsService,
    private router: Router,
    private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.getUser();
    this.setNewNotificationStatus();
    this.initializeForm();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  getUser() {
    const sub = this.accountService.user$.subscribe({
      next: (response: any) => {
        this.user = response;
      }
    });
    this.subscriptions.push(sub);
  }

  setNewNotificationStatus() {
    const sub = this.sharedService.newNotifications$.subscribe(value => {
      this.newNotifications = value;
    });
    this.subscriptions.push(sub);
  }

  @HostListener('document:click', ['$event'])
  onClick(event: Event): void {
    if (!this.dropdown?.nativeElement.contains(event.target)) {
      this.searchForm.controls['searchString'].setValue('');
    }
  }

  initializeForm() {
    this.searchForm = this.formBuilder.group({
      searchString: ['', [Validators.required]],
      searchForAccounts: []
    })
    const sub = this.searchForm.get('searchString')?.valueChanges.subscribe(
      value => {
        this.search(value, this.searchForAccounts);
      });
    if (sub)
      this.subscriptions.push(sub);
  }

  search(value: string, forAccounts: boolean) {
    this.submitted = true;
    if (this.searchForm && value.length > 0) {
      if (forAccounts) {
        const sub = this.accountService.search(value).subscribe({
          next: (response: any) => {
            this.searchResultAccount = response;
          }
        });
        this.subscriptions.push(sub);
      }
      else {
        const sub = this.restaurantService.search(value).subscribe({
          next: (response: any) => {
            this.searchResultRestaurant = response;
          }
        });
        this.subscriptions.push(sub);
      }
    }
  }

  changeSearchText() {
    if (!this.searchForAccounts) {
      this.searchingFor = 'потребител';
    }
    else {
      this.searchingFor = 'ресторант';
    }
  }

  logout() {
    if (this.user?.isManager) {
      this.managerService.logout();
    } else {
      this.employeeService.logout();
    }
  }

  homePage() {
    if (this.user) {
      if (this.user.isManager) {
        this.router.navigateByUrl('/manager');
      }
      else {
        this.router.navigateByUrl('/employee');
      }
    }
    else {
      this.router.navigateByUrl('/');
    }
  }

  employeeSearch() {
    this.router.navigateByUrl('/manager/employees-looking-for-job');
  }

  help() {
    this.router.navigateByUrl('/about');
  }

  allRestaurants() {
    this.router.navigateByUrl('/restaurants');
  }

  schedule() {
    if (this.user?.isManager) {
      this.sharedService.openScheduleManagerModal();
    }
    else if (this.user?.isManager == false) {
      this.sharedService.openScheduleEmployeeModal();
    }
  }

  closeDropdown() {
    const dropdownToggle = document.getElementById('dropdownToggle');
    if (dropdownToggle) {
      dropdownToggle.click();
    }
  }
}
