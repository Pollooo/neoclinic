import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { LocalStorageService } from '../services/local-storage.service';

export const authGuard: CanActivateFn = (route, state) => {
  const localStorageService = inject(LocalStorageService);
  const router = inject(Router);

  if (localStorageService.isAuthenticated()) {
    return true;
  }

  // Redirect to login page
  router.navigate(['/admin/login']);
  return false;
};

