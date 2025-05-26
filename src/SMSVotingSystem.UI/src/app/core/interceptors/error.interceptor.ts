import { HttpInterceptorFn } from '@angular/common/http';
import { ErrorService } from '../services/error.service';
import { inject } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const errorService = inject(ErrorService);
  const authService = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error) => {
      let errorMessage = 'An unknown error occurred';

      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = `Error: ${error.error.message}`;
      } else {
        // Server-side error
        if (error.status === 401) {
          // Unauthorized - clear any stored tokens and redirect to login
          authService.logout();
          router.navigate(['/auth/login']);
          errorMessage = 'Your session has expired. Please log in again.';
        } else if (error.status === 403) {
          errorMessage = 'You do not have permission to perform this action.';
        } else if (error.status === 404) {
          errorMessage = 'The requested resource was not found.';
        } else if (error.error && error.error.error && error.error.error.message) {
          errorMessage = error.error.error.message;
        } else {
          errorMessage = `Error ${error.status}: ${error.message}`;
        }
      }
      
      errorService.setError(errorMessage);
      return throwError(() => new Error(errorMessage));

      // if (error.status === 401) {
      //   authService.logout();
      //   router.navigate(['/auth/login']);
      // } else if (error.status === 403) {
      //   router.navigate(['/dashboard']);
      // } else {
      //   errorService.setError(error.message);
      // }
      // return throwError(error);
    })
  );

  return next(req);
};
