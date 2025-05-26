import { formatDate } from '@angular/common';
import { Component, OnInit } from '@angular/core';
// import bootstrap from 'bootstrap';
import { ErrorService } from '../../../../core/services/error.service';
import { Election, UpdateElectionDto, CreateElectionDto } from '../../models/election.model';
import { ElectionService } from '../../services/election.service';

declare var bootstrap: any;
@Component({
  selector: 'app-elections-page',
  templateUrl: './elections-page.component.html',
  styleUrl: './elections-page.component.scss'
})
export class ElectionsPageComponent implements OnInit {
  elections: Election[] = [];
  upcomingElections: Election[] = [];
  pastElections: Election[] = [];
  activeElection: Election | null = null;
  
  selectedElection: Election | null = null;
  electionToDelete: Election | null = null;
  
  modalRef: any;
  confirmationModalRef: any;
  
  constructor(
    private electionService: ElectionService,
    private errorService: ErrorService
  ) { }
  
  ngOnInit(): void {
    this.loadElections();
  }
  
  loadElections(): void {
    this.electionService.getElections().subscribe({
      next: (elections) => {
        this.elections = elections;
        this.categorizeElections();
      },
      error: (error) => {
        this.errorService.setError('Failed to load elections: ' + error.message);
      }
    });
  }
  
  categorizeElections(): void {
    const now = new Date();
    
    // Find active election
    this.activeElection = this.elections.find(e => e.isActive) || null;
    
    // Filter upcoming elections (not active and end date is in the future)
    this.upcomingElections = this.elections.filter(e => {
      const endDate = new Date(e.endDate);
      return !e.isActive && endDate >= now;
    });
    
    // Filter past elections (not active and end date is in the past)
    this.pastElections = this.elections.filter(e => {
      const endDate = new Date(e.endDate);
      return !e.isActive && endDate < now;
    });
    
    // Sort by date
    this.upcomingElections.sort((a, b) => 
      new Date(a.startDate).getTime() - new Date(b.startDate).getTime()
    );
    
    this.pastElections.sort((a, b) => 
      new Date(b.endDate).getTime() - new Date(a.endDate).getTime()
    );
  }
  
  openCreateModal(): void {
    this.selectedElection = null;
    this.modalRef = new bootstrap.Modal(document.getElementById('electionModal')!);
    this.modalRef.show();
  }
  
  openEditModal(election: Election): void {
    this.selectedElection = { ...election };
    this.modalRef = new bootstrap.Modal(document.getElementById('electionModal')!);
    this.modalRef.show();
  }
  
  openDeleteModal(election: Election): void {
    this.electionToDelete = election;
    this.confirmationModalRef = new bootstrap.Modal(document.getElementById('confirmationModal')!);
    this.confirmationModalRef.show();
  }
  
  saveElection(formData: any): void {
    if (this.selectedElection) {
      // Update existing election
      const updateDto: UpdateElectionDto = {
        title: formData.title,
        description: formData.description,
        startDate: formData.startDate,
        endDate: formData.endDate
      };
      
      this.electionService.updateElection(this.selectedElection.id, updateDto).subscribe({
        next: () => {
          this.loadElections();
          this.modalRef.hide();
        },
        error: (error) => {
          this.errorService.setError('Failed to update election: ' + error.message);
        }
      });
    } else {
      // Create new election
      const createDto: CreateElectionDto = {
        title: formData.title,
        description: formData.description,
        startDate: formData.startDate,
        endDate: formData.endDate
      };
      
      this.electionService.createElection(createDto).subscribe({
        next: () => {
          this.loadElections();
          this.modalRef.hide();
        },
        error: (error) => {
          this.errorService.setError('Failed to create election: ' + error.message);
        }
      });
    }
  }
  
  cancelElectionForm(): void {
    this.modalRef.hide();
  }
  
  confirmDelete(): void {
    if (this.electionToDelete) {
      this.electionService.deleteElection(this.electionToDelete.id).subscribe({
        next: () => {
          this.loadElections();
          this.electionToDelete = null;
        },
        error: (error) => {
          this.errorService.setError('Failed to delete election: ' + error.message);
        }
      });
    }
  }
  
  cancelDelete(): void {
    this.electionToDelete = null;
  }
  
  activateElection(id: number): void {
    this.electionService.activateElection(id).subscribe({
      next: () => {
        this.loadElections();
      },
      error: (error) => {
        this.errorService.setError('Failed to activate election: ' + error.message);
      }
    });
  }
  
  deactivateElection(id: number): void {
    this.electionService.deactivateElection(id).subscribe({
      next: () => {
        this.loadElections();
      },
      error: (error) => {
        this.errorService.setError('Failed to deactivate election: ' + error.message);
      }
    });
  }
  
  formatDate(date: string): string {
    return date ? formatDate(date, 'MMM d, y, h:mm a', 'en-US') : 'N/A';
  }
}
