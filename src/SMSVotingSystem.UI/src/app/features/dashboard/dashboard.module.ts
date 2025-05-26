import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardPageComponent } from './pages/dashboard-page/dashboard-page.component';
import { VotersPageComponent } from '../voters/pages/voters-page/voters-page.component';
import { SharedModule } from "../../shared/shared.module";

@NgModule({
  declarations: [
    DashboardPageComponent,
    // VotersPageComponent,
    // VotersDetailPageComponent
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    SharedModule
]
})
export class DashboardModule { }
