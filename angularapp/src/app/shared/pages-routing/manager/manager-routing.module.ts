import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManagerLoggedViewComponent } from '../../logged-pages/manager-logged-view/manager-logged-view.component';
import { EmployeesLookingForJobComponent } from '../../logged-pages/manager-employee-search/employees-looking-for-job.component';

const routes: Routes = [
  {path: '', component: ManagerLoggedViewComponent},
  {path: 'employees-looking-for-job', component: EmployeesLookingForJobComponent}
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
export class ManagerRoutingModule { }
