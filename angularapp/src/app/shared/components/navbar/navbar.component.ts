import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AccountService } from '../../pages-routing/account/account.service';
import { ManagerService } from '../../pages-routing/manager/manager.service';
import { User } from '../../models/account/user';
import { Router } from '@angular/router';
import { SharedService } from '../../shared.service';
import { EmployeeService } from '../../pages-routing/employee/employee.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SearchResult } from '../../models/account/search-result';

@Component({
  selector: 'nav-top',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  submitted = false;
  user: User | null | undefined;
  fileName: string | undefined;
  searchResult: SearchResult[] = [];
  @ViewChild('searchToggle', {static : false}) searchToggle: ElementRef | undefined;

  constructor(public accountService: AccountService,
    public managerService: ManagerService,
    public employeeService: EmployeeService,
    public sharedService: SharedService,
    public router: Router,
    private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.getUser();
    this.initializeForm();
  }

  getUser() {
    this.accountService.user$.subscribe(user => {
      this.user = user;
      if (this.user && user) {
        if (this.user.isManager) {
          this.managerService.getManager(this.user.email).subscribe({
            next: (response: any) => {
              this.managerService.setManager(response);
            }
          })
        }
        else {
          this.employeeService.getEmployee(this.user.email).subscribe({
            next: (response: any) => {
              this.employeeService.setEmployee(response);
            }
          })
        }
      }
    });
  }

  initializeForm() {
    this.searchForm = this.formBuilder.group({
      searchString: ['', [Validators.required]]
    })

    this.searchForm.get('searchString')?.valueChanges.subscribe(
      value => {
      this.search();
        this.searchToggle?.nativeElement.click();
    });
  }

  search() {
    this.submitted = true;
    if (this.searchForm.valid) {
      this.accountService.search(this.searchForm.value.searchString).subscribe({
        next: (response: any) => {
          this.searchResult = response;
        }
      })
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

  contact() { }

  help() { }

  allRestaurants() {
    this.router.navigateByUrl('/restaurants');
  }

  downloadPDF() {
    if (this.user?.email) {
      this.employeeService.employee$.subscribe({
        next: (response: any) => {
          this.fileName = response.firstName;
        }
      })
      this.employeeService.getPDFFile(this.user.email).subscribe({
        next: (response: Blob) => {
          const blobUrl = window.URL.createObjectURL(response);  // Create a Blob object URL for the downloaded file       
          const a = document.createElement('a');// Create an anchor element and trigger a click to start the download
          a.href = blobUrl;
          a.download = this.fileName + '_cv.pdf';
          document.body.appendChild(a);
          a.click();
          document.body.removeChild(a); // Cleanup: Remove the anchor and revoke the Blob URL
          window.URL.revokeObjectURL(blobUrl);
        },
        error: (error: any) => {
          this.sharedService.showNotification(false, 'Грешка при сваляне', error.message);
          console.log(error);
        }
      });
    }
  }
}
