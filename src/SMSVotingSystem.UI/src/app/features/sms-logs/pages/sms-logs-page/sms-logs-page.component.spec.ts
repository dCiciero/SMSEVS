import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsLogsPageComponent } from './sms-logs-page.component';

describe('SmsLogsPageComponent', () => {
  let component: SmsLogsPageComponent;
  let fixture: ComponentFixture<SmsLogsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SmsLogsPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SmsLogsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
