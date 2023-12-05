import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent {

  constructor(private router: Router) { }

  login() {
    this.router.navigateByUrl('/account/login');
  }

  register() {
    this.router.navigateByUrl('/account/register-employee');
  }
}
