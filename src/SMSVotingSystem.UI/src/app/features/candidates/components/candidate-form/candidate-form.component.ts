import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { Candidate } from '../../models/candidate.model';
import { ElectionService } from '../../../elections/services/election.service';
import { Election } from '../../../elections/models/election.model';
import { dateOfBirthValidator } from '../../../../shared/validators/dob.validator';

@Component({
  selector: 'app-candidate-form',
  templateUrl: './candidate-form.component.html',
  styleUrl: './candidate-form.component.scss',
})
export class CandidateFormComponent {
  @Input() candidate: Candidate | null = null; // = {};
  @Input() isEditMode = false;
  @Output() formSubmitted = new EventEmitter<any>();
  @Output() closed = new EventEmitter<void>();

  @Output() save = new EventEmitter<any>();
  @Output() cancel = new EventEmitter<void>();

  candidateForm: FormGroup;
  isEditing = false;
  elections: Election[] = [];
  election: Election | null = null;

  constructor(private fb: FormBuilder, private electionService: ElectionService) {
    this.getActiveElection();
    this.candidateForm = this.createForm();
  }

  getActiveElection() {
    this.electionService.getActiveElection().subscribe((election) => {
      this.election = election;
      this.candidateForm.patchValue({
        electionId: this.election?.id
      });
    });
  }
  createForm(): FormGroup {
      return this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.pattern(/^[a-zA-Z\s]+$/)]],
        party: ['', [Validators.required, Validators.maxLength(100)]],
        phoneNumber: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(15), Validators.pattern(/^\+?[0-9\s\-\(\)]+$/)]],
        email: ['', [Validators.required, Validators.email, Validators.maxLength(100), Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)]],
        address: ['', [Validators.required, Validators.maxLength(200)]],
        dateOfBirth: ['', [Validators.required, Validators.pattern(/^\d{4}-\d{2}-\d{2}$/), dateOfBirthValidator(21)]],
        // I need the electionId to be set to the active election ID
        // electionId: [this.election?.id, [Validators.required]],

        electionId: [this.election?.id, [Validators.required]],
        isActive: [true]
      });
    }

  submit() {
    //log the form values
    console.log(this.candidateForm.value);
    if (this.candidateForm.invalid) {
      return;
    }
    const formValues = this.candidateForm.value;
    this.candidate = {
      name: formValues.name,
      description: '',
      shortCode: '',
      position: '',
      party: formValues.party,
      phoneNumber: formValues.phoneNumber,
      email: formValues.email,
      address: formValues.address,
      dateOfBirth: formValues.dateOfBirth,
      electionId: this.election?.id ?? 0, // Use the active election ID, fallback to 0
      isActive: formValues.isActive
    };
    this.save.emit(this.candidate);

    console.log('Candidate Submitted:', this.candidate);
  }

  close() {
    this.closed.emit();
  }

  // candidate = { name: '', party: '' };

  // onSubmit(form: NgForm) {
  //   if (form.valid) {
  //     console.log('Candidate Submitted:', this.candidate);
  //     form.resetForm();
  //   }
  // }
}
