import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  private errorMessageSubject = new BehaviorSubject<string | null>(null);
  public errorMessage$ = this.errorMessageSubject.asObservable();

  setError(message: string): void {
    this.errorMessageSubject.next(message);
  }

  clearError(): void {
    this.errorMessageSubject.next(null);
  }
}
