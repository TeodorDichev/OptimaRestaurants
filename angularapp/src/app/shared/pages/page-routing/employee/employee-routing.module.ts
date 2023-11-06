import { NgModule } from '@angular/core';
import { EmployeeLoggedViewComponent } from '../../employee-logged-view/employee-logged-view/employee-logged-view.component';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {path: '', component: EmployeeLoggedViewComponent}
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class EmployeeRoutingModule { }
