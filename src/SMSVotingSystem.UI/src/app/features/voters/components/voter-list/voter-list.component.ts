import { formatDate } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ErrorService } from '../../../../core/services/error.service';
import * as bootstrap from 'bootstrap';
import { Voter, UpdateVoterDto, CreateVoterDto } from '../../models/voter.model';
import { VoterService } from '../../services/voter.service';

@Component({
  selector: 'app-voter-list',
  templateUrl: './voter-list.component.html',
  styleUrl: './voter-list.component.scss'
})
export class VoterListComponent implements OnInit {
  voters: Voter[] = [];
  filteredVoters: Voter[] = [];
  selectedVoter: Voter | null = null;
  voterToDelete: Voter | null = null;
  searchTerm = '';
  modalRef: any;
  confirmationModalRef: any;
  
  constructor(
    private voterService: VoterService,
    private errorService: ErrorService
  ) { }
  
  ngOnInit(): void {
    this.loadVoters();
  }
  
  loadVoters(): void {
    this.voterService.getVoters().subscribe({
      next: (voters) => {
        this.voters = voters;
        this.applyFilter();
      },
      error: (error) => {
        this.errorService.setError('Failed to load voters: ' + error.message);
      }
    });
  }
  
  applyFilter(): void {
    if (!this.searchTerm.trim()) {
      this.filteredVoters = [...this.voters];
      return;
    }
    
    const search = this.searchTerm.toLowerCase();
    this.filteredVoters = this.voters.filter((voter) => 
      voter.name.toLowerCase().includes(search) || 
      voter.phoneNumber.includes(search)
    );
  }
  
  openAddModal(): void {
    this.selectedVoter = null;
    // this.modalRef = new bootstrap.Modal(document.getElementById('voterModal')!);
    // this.modalRef.show();
    const modalElement = document.getElementById('voterModal');
    if (modalElement) {
        this.modalRef = new bootstrap.Modal(modalElement);
        this.modalRef.show();
    } else {
        console.error('Modal element not found');
    }
    
  }
  
  openEditModal(voter: Voter): void {
    this.selectedVoter = { ...voter };
    // this.modalRef = new bootstrap.Modal(document.getElementById('voterModal'));
    // this.modalRef.show();
    const modalElement = document.getElementById('voterModal');
    if (modalElement) {
        this.modalRef = new bootstrap.Modal(modalElement);
        this.modalRef.show();
    } else {
        console.error('Modal element not found');
    }
  }
  
  openDeleteModal(voter: Voter): void {
    this.voterToDelete = voter;
    // this.confirmationModalRef = new bootstrap.Modal(document.getElementById('confirmationModal'));
    // this.confirmationModalRef.show();
    const modalElement = document.getElementById('confirmationModal');
    if (modalElement) {
        this.confirmationModalRef = new bootstrap.Modal(modalElement);
        this.confirmationModalRef.show();
    } else {
        console.error('Confirmation modal element not found');
    }
  }
  
  confirmDelete(): void {
    if (this.voterToDelete) {
      this.voterService.deleteVoter(this.voterToDelete.id).subscribe({
        next: () => {
          this.loadVoters();
          this.voterToDelete = null;
        },
        error: (error) => {
          this.errorService.setError('Failed to delete voter: ' + error.message);
        }
      });
    }
  }
  
  cancelDelete(): void {
    this.voterToDelete = null;
  }
  
  saveVoter(formData: any): void {
    if (this.selectedVoter) {
      // Update existing voter
      const updateDto: UpdateVoterDto = {
        name: formData.name,
        isRegistered: formData.isRegistered
      };
      
      this.voterService.updateVoter(this.selectedVoter.id, updateDto).subscribe({
        next: () => {
          this.loadVoters();
          this.modalRef.hide();
        },
        error: (error) => {
          this.errorService.setError('Failed to update voter: ' + error.message);
        }
      });
    } else {
      // Create new voter
      const createDto: CreateVoterDto = {
        name: formData.name,
        phoneNumber: formData.phoneNumber
      };
      
      this.voterService.createVoter(createDto).subscribe({
        next: () => {
          this.loadVoters();
          this.modalRef.hide();
        },
        error: (error) => {
          this.errorService.setError('Failed to create voter: ' + error.message);
        }
      });
    }
  }
  
  cancelVoterForm(): void {
    this.modalRef.hide();
  }
  
  formatDate(date: string): string {
    return date ? formatDate(date, 'MMM d, y, h:mm a', 'en-US') : 'N/A';
  }
}
