import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ValidationMessagesComponent } from './components/errors/validation-messages/validation-messages.component';
import { RouterModule } from '@angular/router';
import { NotFoundComponent } from './components/errors/not-found/not-found.component';

@NgModule({
  declarations: [
    ValidationMessagesComponent,
    NotFoundComponent
  ],
  imports: [
    CommonModule,
    RouterModule
  ],
  exports: [
    RouterModule
  ]
})
export class SharedModule { }
