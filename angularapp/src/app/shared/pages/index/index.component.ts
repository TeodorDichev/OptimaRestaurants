import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ManagerService } from '../page-routing/manager/manager.service'; // for testing only, remove later
@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css',
    '../../../app.component.css']
})
export class IndexComponent {


  constructor(private router: Router,
    private managerService: ManagerService) { }


  login() {
    this.router.navigateByUrl('/account/login');
  }

  register() {
    this.router.navigateByUrl('/account/register-employee');
  }
}
