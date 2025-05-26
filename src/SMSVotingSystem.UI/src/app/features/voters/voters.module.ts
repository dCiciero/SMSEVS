import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { VotersRoutingModule } from './voters-routing.module';
import { VotersPageComponent } from './pages/voters-page/voters-page.component';
import { VoterDetailPageComponent } from './pages/voter-detail-page/voter-detail-page.component';
import { VoterListComponent } from './components/voter-list/voter-list.component';
import { VoterFormComponent } from './components/voter-form/voter-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from "../../shared/shared.module";


@NgModule({
  declarations: [
    VotersPageComponent,
    VoterDetailPageComponent,
    VoterListComponent,
    VoterFormComponent
  ],
  imports: [
    CommonModule,
    VotersRoutingModule,
    ReactiveFormsModule,
    SharedModule
]
})
export class VotersModule { }
