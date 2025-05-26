import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VoterDetailPageComponent } from './voter-detail-page.component';

describe('VoterDetailPageComponent', () => {
  let component: VoterDetailPageComponent;
  let fixture: ComponentFixture<VoterDetailPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [VoterDetailPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VoterDetailPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
