import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ElectionsRoutingModule } from './elections-routing.module';
import { ElectionsPageComponent } from './pages/elections-page/elections-page.component';
import { ElectionListComponent } from './components/election-list/election-list.component';
import { ElectionFormComponent } from './components/election-form/election-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from "../../shared/shared.module";


@NgModule({
  declarations: [
    ElectionsPageComponent,
    ElectionListComponent,
    ElectionFormComponent
  ],
  imports: [
    CommonModule,
    ElectionsRoutingModule,
    ReactiveFormsModule,
    SharedModule
]
})
export class ElectionsModule { }
