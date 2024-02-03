import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IndexComponent } from './shared/account-pages/index/index.component';
import { NotFoundComponent } from './shared/components/errors/not-found/not-found.component';
import { AuthorizationGuard } from './shared/guards/unlogged-guard/authorization.guard';
import { LoggedAuthorizationGuard } from './shared/guards/logged-guard/logged-authorization.guard';

const routes: Routes = [
  { path: '', canActivate: [LoggedAuthorizationGuard], component: IndexComponent },
  { path: 'account', canActivate: [LoggedAuthorizationGuard], loadChildren: () => import('./shared/pages-routing/account/account.module').then(module => module.AccountModule) },
  { path: 'manager', canActivate: [AuthorizationGuard], loadChildren: () => import('./shared/pages-routing/manager/manager.module').then(module => module.ManagerModule) },
  { path: 'employee', canActivate: [AuthorizationGuard], loadChildren: () => import('./shared/pages-routing/employee/employee.module').then(module => module.EmployeeModule) },
  { path: 'review', loadChildren: () => import('./shared/pages-routing/review/reviews.module').then(module => module.ReviewsModule) },
  { path: 'restaurants', loadChildren: () => import('./shared/pages-routing/restaurants/restaurants.module').then(module => module.RestaurantsModule) },
  { path: 'about', loadChildren: () => import('./shared/pages-routing/about/about.module').then(module => module.AboutModule) },
  { path: '**', component: NotFoundComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
