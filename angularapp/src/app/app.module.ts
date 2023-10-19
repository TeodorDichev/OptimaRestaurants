import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './shared/pages/login/login.component';
import { RegisterManagerComponent } from './shared/pages/registerManager/registerManager.component';
import { IndexComponent } from './shared/pages/index/index.component';
import { RegisterEmployeeComponent } from './shared/pages/registerEmployee/registerEmployee.component';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { SharedModule } from './shared/shared.module';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterManagerComponent,
    IndexComponent,
    RegisterEmployeeComponent,
    NavbarComponent
  ],
  imports: [
    BrowserModule, 
    SharedModule,
    HttpClientModule, 
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
