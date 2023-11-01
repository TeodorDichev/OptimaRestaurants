import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManagerLoggedViewComponent } from '../../manager-logged-view/manager-logged-view/manager-logged-view.component';

const routes: Routes = [
  {path: '', component: ManagerLoggedViewComponent}
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
