import { Component } from '@angular/core';

@Component({
  selector: 'app-signupemployee',
  templateUrl: './signupemployee.component.html',
  styleUrls: ['./signupemployee.component.css']
})
export class SignupEmployeeComponent {
  isText: boolean = false;
  type: string = "Password";
  eyeIcon: string = "fa-eye-slash";
  hideShowPass(){
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }
}
