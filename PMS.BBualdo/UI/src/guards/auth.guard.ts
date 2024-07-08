import { CanActivateChildFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { map, Observable } from 'rxjs';

export const authGuard: CanActivateChildFn = (): Observable<boolean> => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.getCurrentUser().pipe(
    map((user) => {
      if (!user) {
        router.navigate(['/login']);
        return false;
      }
      return true;
    }),
  );
};
