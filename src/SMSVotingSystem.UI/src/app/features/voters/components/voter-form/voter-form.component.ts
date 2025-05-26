import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Voter } from '../../models/voter.model';

@Component({
  selector: 'app-voter-form',
  templateUrl: './voter-form.component.html',
  styleUrl: './voter-form.component.scss'
})
export class VoterFormComponent implements OnInit {
  @Input() voter: Voter | null = null;
  @Output() save = new EventEmitter<any>();
  @Output() cancel = new EventEmitter<void>();
  
  voterForm: FormGroup;
  isEditing = false;
  
  constructor(private fb: FormBuilder) {
    this.voterForm = this.createForm();
  }
  
  ngOnInit(): void {
    this.isEditing = !!this.voter;
    if (this.voter) {
      this.voterForm.patchValue({
        name: this.voter.name,
        phoneNumber: this.voter.phoneNumber,
        isRegistered: this.voter.isRegistered
      });
      
      // Disable phone number editing in edit mode
      this.voterForm.get('phoneNumber')?.disable();
    }
  }
  
  createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[0-9\s\-\(\)]+$/)]],
      isRegistered: [true]
    });
  }
  
  onSubmit(): void {
    if (this.voterForm.invalid) {
      return;
    }
    
    const formData = this.voterForm.getRawValue();
    this.save.emit(formData);
  }
  
  onCancel(): void {
    this.cancel.emit();
  }
}
