import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
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
import { take } from 'rxjs';

@Component({
  selector: 'nav-top',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
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

  getUser() {
    this.accountService.user$.pipe(take(1)).subscribe({
      next: (response: any) => {
        this.user = response;
      }
    });
  }

  setNewNotificationStatus() {
    this.sharedService.newNotifications$.subscribe(value => {
      this.newNotifications = value;
    });
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
    this.searchForm.get('searchString')?.valueChanges.subscribe(
      value => {
        this.search(value, this.searchForAccounts);
      });
  }

  search(value: string, forAccounts: boolean) {
    this.submitted = true;
    if (this.searchForm && value.length > 0) {
      if (forAccounts) {
        this.accountService.search(value).pipe(take(1)).subscribe({
          next: (response: any) => {
            this.searchResultAccount = response;
          }
        })
      }
      else {
        this.restaurantService.search(value).pipe(take(1)).subscribe({
          next: (response: any) => {
            this.searchResultRestaurant = response;
          }
        })
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

  contact() {
    this.router.navigateByUrl('/contact');
  }

  help() {
    this.router.navigateByUrl('/about');
  }

  allRestaurants() {
    this.router.navigateByUrl('/restaurants');
  }

  schedule() {
    
  }
}
