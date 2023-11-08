import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManagerLoggedViewComponent } from '../../manager-logged-view/manager-logged-view.component';

const routes: Routes = [
  {path: '', component: ManagerLoggedViewComponent},
  {path: 'employees-looking-for-job', component: ManagerLoggedViewComponent} // TODO -> employees-looking-for-job component
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
