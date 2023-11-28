import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared.module';
import { RestaurantsRoutingModule } from './restaurants-routing.module';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RestaurantsRoutingModule,
    SharedModule
  ]
})

export class RestaurantsModule { }
