import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IndexComponent } from './shared/pages/index/index.component';
import { NotFoundComponent } from './shared/components/errors/not-found/not-found.component';
import { AuthorizationGuard } from './shared/guards/authorization.guard';

const routes: Routes = [
  {path: '', component: IndexComponent},
  {path: 'account', loadChildren:() => import('./shared/pages/page-routing/account/account.module').then(module => module.AccountModule)},
  {path: 'manager', canActivate:[AuthorizationGuard], loadChildren:() => import('./shared/pages/page-routing/manager/manager.module').then(module => module.ManagerModule) },
  {path: '**', component: NotFoundComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { 
  
}
