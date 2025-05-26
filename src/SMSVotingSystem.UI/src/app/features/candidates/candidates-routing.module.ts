import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CandidatesPageComponent } from './pages/candidates-page/candidates-page.component';

const routes: Routes = [
  { path: '', component: CandidatesPageComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CandidatesRoutingModule { }
