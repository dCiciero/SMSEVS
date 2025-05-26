import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Election } from '../../models/election.model';

@Component({
  selector: 'app-election-form',
  templateUrl: './election-form.component.html',
  styleUrl: './election-form.component.scss'
})
export class ElectionFormComponent implements OnInit {
  @Input() election: Election | null = null;
  @Output() save = new EventEmitter<any>();
  @Output() cancel = new EventEmitter<void>();
  
  electionForm: FormGroup;
  isEditing = false;
  
  constructor(private fb: FormBuilder) {
    this.electionForm = this.createForm();
  }
  
  ngOnInit(): void {
    this.isEditing = !!this.election;
    if (this.election) {
      // Format dates for datetime-local input
      const startDate = new Date(this.election.startDate);
      const endDate = new Date(this.election.endDate);
      
      this.electionForm.patchValue({
        title: this.election.title,
        description: this.election.description,
        startDate: this.formatDateForInput(startDate),
        endDate: this.formatDateForInput(endDate)
      });
    }
  }
  
  createForm(): FormGroup {
    return this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: [''],
      startDate: ['', [Validators.required]],
      endDate: ['', [Validators.required]]
    }, { validators: this.dateRangeValidator });
  }
  
  dateRangeValidator(group: FormGroup) {
    const startDate = group.get('startDate')?.value;
    const endDate = group.get('endDate')?.value;
    
    if (startDate && endDate) {
      const start = new Date(startDate);
      const end = new Date(endDate);
      
      if (end <= start) {
        return { endDateAfterStart: true };
      }
    }
    
    return null;
  }
  
  formatDateForInput(date: Date): string {
    return date.toISOString().slice(0, 16);
  }
  
  onSubmit(): void {
    if (this.electionForm.invalid) {
      return;
    }
    
    const formData = this.electionForm.value;
    this.save.emit(formData);
  }
  
  onCancel(): void {
    this.cancel.emit();
  }
}
