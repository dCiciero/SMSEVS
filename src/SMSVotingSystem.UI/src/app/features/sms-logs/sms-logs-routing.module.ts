import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SmsLogsPageComponent } from './pages/sms-logs-page/sms-logs-page.component';

const routes: Routes = [
  {
    path: '',
    component: SmsLogsPageComponent
  },
  {
    path: 'details/:id',
    component: SmsLogsPageComponent,
    data: { mode: 'details' }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SmsLogsRoutingModule { }
