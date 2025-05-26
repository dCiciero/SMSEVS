import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ErrorService } from '../../../core/services/error.service';

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrl: './alert.component.scss'
})
export class AlertComponent implements OnInit, OnDestroy {
  message: string | null = null;
  private subscription: Subscription | null = null;

  constructor(private errorService: ErrorService) {}

  ngOnInit(): void {
    this.subscription = this.errorService.errorMessage$.subscribe(
      message => {
        this.message = message;
        
        if (message) {
          // Auto-close alert after 5 seconds
          setTimeout(() => this.close(), 5000);
        }
      }
    );
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  close(): void {
    this.message = null;
    this.errorService.clearError();
  }
}
