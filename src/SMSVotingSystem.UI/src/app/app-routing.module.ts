import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardPageComponent } from './features/dashboard/pages/dashboard-page/dashboard-page.component';
import { VotersPageComponent } from './features/voters/pages/voters-page/voters-page.component';
import { CandidateListComponent } from './features/candidates/components/candidate-list/candidate-list.component';
import { ElectionListComponent } from './features/elections/components/election-list/election-list.component';
import { ResultsChartComponent } from './features/results/components/results-chart/results-chart.component';
import { SmsLogsPageComponent } from './features/sms-logs/pages/sms-logs-page/sms-logs-page.component';
import { authGuard } from './core/auth/auth.guard';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'dashboard',
    loadChildren: () => import('./features/dashboard/dashboard.module').then(m => m.DashboardModule)
    // canActivate: [authGuard]
  },
  {
    path: 'voters',
    loadChildren: () => import('./features/voters/voters.module').then(m => m.VotersModule),
    // canActivate: [authGuard]
  },
  {
    path: 'candidates',
    loadChildren: () => import('./features/candidates/candidates.module').then(m => m.CandidatesModule),
    // canActivate: [authGuard]
  },
  {
    path: 'elections',
    loadChildren: () => import('./features/elections/elections.module').then(m => m.ElectionsModule),
    // canActivate: [authGuard]
  },
  {
    path: 'results',
    loadChildren: () => import('./features/results/results.module').then(m => m.ResultsModule),
    // canActivate: [authGuard]
  },
  {
    path: 'sms-logs',
    loadChildren: () => import('./features/sms-logs/sms-logs.module').then(m => m.SmsLogsModule),
    // canActivate: [authGuard]
  },
  // Wildcard route for 404
  { path: '**', redirectTo: 'dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
