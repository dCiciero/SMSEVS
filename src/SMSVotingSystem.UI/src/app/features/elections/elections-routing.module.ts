import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ElectionsPageComponent } from './pages/elections-page/elections-page.component';
import { authGuard } from '../../core/auth/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: ElectionsPageComponent,
    // canActivate: [authGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ElectionsRoutingModule { }
