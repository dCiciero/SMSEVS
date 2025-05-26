import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VoterFormComponent } from './voter-form.component';

describe('VoterFormComponent', () => {
  let component: VoterFormComponent;
  let fixture: ComponentFixture<VoterFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [VoterFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VoterFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
