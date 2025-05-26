import { Component, OnInit, ViewChild } from '@angular/core';
import { CandidateService } from '../../../candidates/services/candidate.service';
import { Election } from '../../../elections/models/election.model';
import { ElectionService } from '../../../elections/services/election.service';
import { VoteResult } from '../../../results/models/result.model';
import { SmsLogService } from '../../../sms-logs/services/sms-log.service';
import { VoterService } from '../../../voters/services/voter.service';
import { VoteService } from '../../../results/services/vote.service';

@Component({
  selector: 'app-dashboard-page',
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.scss',
})
export class DashboardPageComponent implements OnInit {
  voterCount = 0;
  candidateCount = 0;
  activeElection: Election | null = null;
  voteResults: VoteResult[] = [];
  recentSmsLogs: any[] = [];
  isLoading = true;

  // Simple properties for now - we'll handle the chart in the HTML

  constructor(
    private voterService: VoterService,
    private candidateService: CandidateService,
    private electionService: ElectionService,
    private voteService: VoteService,
    private smsLogService: SmsLogService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.isLoading = true;

    // Get voter count
    this.voterService.getVoters().subscribe((voters) => {
      console.log('getting voteres');
      this.voterCount = voters.length;
    });

    // Get candidate count
    this.candidateService.getCandidates().subscribe((candidates) => {
      this.candidateCount = candidates.length;
    });

    // Get active election and vote results
    this.electionService.getActiveElection().subscribe({
      next: (election) => {
        this.activeElection = election;

        if (election) {
          this.voteService.getVoteResults(election.id).subscribe((results) => {
            this.voteResults = results;
          });
        }
      },
      error: () => {
        this.activeElection = null;
      },
      complete: () => {
        this.isLoading = false;
      },
    });

    // Get recent SMS logs
    this.smsLogService.getRecentLogs(5).subscribe((logs) => {
      this.recentSmsLogs = logs;
    });
  }
}
