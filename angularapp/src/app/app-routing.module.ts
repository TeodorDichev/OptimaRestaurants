import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IndexComponent } from './shared/account-pages/index/index.component';
import { NotFoundComponent } from './shared/components/errors/not-found/not-found.component';
import { AuthorizationGuard } from './shared/guards/authorization.guard';
import { LoggedAuthorizationGuard } from './shared/guards/logged-guard/logged-authorization.guard';

const routes: Routes = [
  {path: '', canActivate:[LoggedAuthorizationGuard], component: IndexComponent},
  {path: 'account', canActivate:[LoggedAuthorizationGuard], loadChildren:() => import('./shared/pages/account/account.module').then(module => module.AccountModule)},
  {path: 'manager', canActivate:[AuthorizationGuard], loadChildren:() => import('./shared/pages/manager/manager.module').then(module => module.ManagerModule) },
  {path: 'employee', canActivate:[AuthorizationGuard], loadChildren:() => import('./shared/pages/employee/employee.module').then(module => module.EmployeeModule) },
  {path: '**', component: NotFoundComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { 
  
}
