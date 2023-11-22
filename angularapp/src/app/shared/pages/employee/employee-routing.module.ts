import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmployeeLoggedViewComponent } from '../../logged-pages/employee-logged-view/employee-logged-view.component';

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
