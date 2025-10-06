import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const session = auth.currentUser();
  if (session) {
    return true;
  }
  router.navigate(['/login'], { queryParams: { returnUrl: router.url } });
  return false;
};
