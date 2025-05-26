import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SmsLogsRoutingModule } from './sms-logs-routing.module';
import { SmsLogsPageComponent } from './pages/sms-logs-page/sms-logs-page.component';
import { LogsTableComponent } from './components/logs-table/logs-table.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    SmsLogsPageComponent,
    LogsTableComponent
  ],
  imports: [
    CommonModule,
    SmsLogsRoutingModule,
    ReactiveFormsModule,
    FormsModule
  ]
})
export class SmsLogsModule { }
