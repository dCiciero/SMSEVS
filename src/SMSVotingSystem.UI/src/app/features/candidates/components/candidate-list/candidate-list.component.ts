import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Candidate } from '../../models/candidate.model';
import * as bootstrap from 'bootstrap';
import { CandidateService } from '../../services/candidate.service';
import { ErrorService } from '../../../../core/services/error.service';

@Component({
  selector: 'app-candidate-list',
  templateUrl: './candidate-list.component.html',
  styleUrl: './candidate-list.component.scss',
})
export class CandidateListComponent {
  @Input() candidates2: any[] = [];
  @Output() add = new EventEmitter<void>();
  @Output() edit = new EventEmitter<any>();
  @Output() remove = new EventEmitter<any>();
  // candidates = [
  //   { name: 'John Doe', party: 'Independent' },
  //   { name: 'Jane Smith', party: 'Democratic' }
  // ];
  candidates: Candidate[] = [];
  filteredCandidates: Candidate[] = [];
  selectedCandidate: Candidate | null = null;
  candidateToDelete: Candidate | null = null;
  searchTerm = '';
  modalRef: any;
  isEditMode = false;
  showFormModal = false;
  confirmationModalRef: any;

  constructor(
    private candidateService: CandidateService,
    private errorService: ErrorService
  ) // private modalService: NgbModal
  {}

  ngOnInit() {
    this.loadCandidates();
  }

  loadCandidates() {
    this.candidateService.getCandidates().subscribe((data) => {
      this.candidates = data;
    });
  }

  saveCandidate(candidate: any) {
    if (this.isEditMode) {
      this.candidateService
        .updateCandidate(candidate.id, candidate)
        .subscribe({
          next: () => {
            this.loadCandidates();
            this.modalRef.hide();
          },
          error: (error) => {
            this.errorService.setError(
              'Failed to update candidate: ' + error.message
            );
          },
        });
    } else {
      this.candidateService
        .createCandidate(candidate)
        .subscribe({
          next: () => {
            this.loadCandidates();
            this.modalRef.hide();
          },
          error: (error) => {
            this.errorService.setError(
              'Failed to create candidate: ' + error.message
            );
          },
        });

    }
    // this.closeModal();
    this.modalRef.hide();
  }

  deleteCandidate(candidate: any) {
    if (confirm(`Delete ${candidate.name}?`)) {
      this.candidateService
        .deleteCandidate(candidate.id)
        .subscribe({
          next: () => {
            this.loadCandidates();
            this.candidateToDelete = null;
          },
          error: (error) => {
            this.errorService.setError(
              'Failed to delete candidate: ' + error.message
            );
          },
        });
    }
  }

  // openAddModal(): void {
  //     this.selectedCandidates = null;
  //     // this.modalRef = new bootstrap.Modal(document.getElementById('voterModal')!);
  //     // this.modalRef.show();
  //     const modalElement = document.getElementById('voterModal');
  //     if (modalElement) {
  //         this.modalRef = new bootstrap.Modal(modalElement);
  //         this.modalRef.show();
  //     } else {
  //         console.error('Modal element not found');
  //     }

  //   }

  applyFilter(): void {
    if (!this.searchTerm.trim()) {
      this.filteredCandidates = [...this.candidates];
      return;
    }

    const search = this.searchTerm.toLowerCase();
    this.filteredCandidates = this.candidates.filter(
      (candidate) => candidate.name.toLowerCase().includes(search) //||
      // candidate.phoneNumber.includes(search)
    );
  }

  openAddModal(): void {
    this.selectedCandidate = null;
    // this.modalRef = new bootstrap.Modal(document.getElementById('voterModal')!);
    // this.modalRef.show();
    const modalElement = document.getElementById('candidateModal');
    if (modalElement) {
      this.modalRef = new bootstrap.Modal(modalElement);
      this.modalRef.show();
    } else {
      console.error('Modal element not found');
    }
  }

  openEditModal(candidate: Candidate): void {
    this.selectedCandidate = { ...candidate };
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

  openDeleteModal(candidate: Candidate): void {
    this.candidateToDelete = candidate;
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
    if (this.candidateToDelete) {
      this.candidateService
        .deleteCandidate(this.candidateToDelete.id!)
        .subscribe({
          next: () => {
            this.loadCandidates();
            this.candidateToDelete = null;
          },
          error: (error) => {
            this.errorService.setError(
              'Failed to delete voter: ' + error.message
            );
          },
        });
    }
  }

  cancelDelete(): void {
    this.candidateToDelete = null;
  }

  cancelCandidateForm(): void {
    this.modalRef.hide();
  }
}
