import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VotersPageComponent } from './pages/voters-page/voters-page.component';
import { authGuard } from '../../core/auth/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: VotersPageComponent,
    // canActivate: [authGuard],
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class VotersRoutingModule { }
