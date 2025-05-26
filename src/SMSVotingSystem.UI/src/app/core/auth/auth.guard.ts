import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  console.log('authGuard', route, state);
  if (!authService.isAuthenticated()) {
    router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  if (route.data['roles'] && route.data['roles'].length > 0) {
    const hasRole = route.data['roles'].some(
      (role:string) => authService.hasRole(role)
    )
    if (!hasRole) {
      // router.navigate(['/access-denied']);
      router.navigate(['/dashboard']);
      return false;
    }
  }
  return true;
};
