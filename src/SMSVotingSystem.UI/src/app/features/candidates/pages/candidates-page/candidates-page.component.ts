import { Component } from '@angular/core';
import { CandidateService } from '../../services/candidate.service';

@Component({
  selector: 'app-candidates-page',
  templateUrl: './candidates-page.component.html',
  styleUrl: './candidates-page.component.scss'
})
export class CandidatesPageComponent {
  // candidates: any[] = [];
  // selectedCandidate: any = {};
  // isEditMode = false;
  // showFormModal = false;

  // constructor(private candidateService: CandidateService) {}

  // ngOnInit() {
  //   this.loadCandidates();
  // }

  // loadCandidates() {
  //   this.candidateService.getCandidates().subscribe(data => {
  //     this.candidates = data;
  //   });
  // }

  // openAddModal() {
  //   this.selectedCandidate = {};
  //   this.isEditMode = false;
  //   this.showFormModal = true;
  // }

  // openEditModal(candidate: any) {
  //   this.selectedCandidate = { ...candidate };
  //   this.isEditMode = true;
  //   this.showFormModal = true;
  // }

  // saveCandidate(candidate: any) {
  //   if (this.isEditMode) {
  //     this.candidateService.updateCandidate(candidate.id, candidate).subscribe(() => this.loadCandidates());
  //   } else {
  //     this.candidateService.createCandidate(candidate).subscribe(() => this.loadCandidates());
  //   }
  //   this.closeModal();
  // }

  // deleteCandidate(candidate: any) {
  //   if (confirm(`Delete ${candidate.name}?`)) {
  //     this.candidateService.deleteCandidate(candidate.id).subscribe(() => this.loadCandidates());
  //   }
  // }

  // closeModal() {
  //   this.showFormModal = false;
  // }
  
  //  candidates = [
  //   {
  //     name: 'John Doe',
  //     party: 'Independent',
  //     registrationDate: new Date(),
  //     active: true
  //   },
  //   {
  //     name: 'Jane Smith',
  //     party: 'Democratic',
  //     registrationDate: new Date(),
  //     active: false
  //   }
  // ];
}
