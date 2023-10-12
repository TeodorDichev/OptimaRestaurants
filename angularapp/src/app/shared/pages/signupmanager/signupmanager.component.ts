import { Component } from '@angular/core';

@Component({
  selector: 'app-signupmanager',
  templateUrl: './signupmanager.component.html',
  styleUrls: ['./signupmanager.component.css']
})
export class SignupManagerComponent {
  isText: boolean = false;
  type: string = "Password";
  eyeIcon: string = "fa-eye-slash";
  hideShowPass(){
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }
}
