import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ElectionsPageComponent } from './elections-page.component';

describe('ElectionsPageComponent', () => {
  let component: ElectionsPageComponent;
  let fixture: ComponentFixture<ElectionsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ElectionsPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ElectionsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
