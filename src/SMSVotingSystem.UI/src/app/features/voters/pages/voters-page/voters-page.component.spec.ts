import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VotersPageComponent } from './voters-page.component';

describe('VotersPageComponent', () => {
  let component: VotersPageComponent;
  let fixture: ComponentFixture<VotersPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [VotersPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VotersPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
