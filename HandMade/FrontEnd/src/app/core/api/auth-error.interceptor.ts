import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { AuthTokenService } from './auth-token.service';

/**
 * Only 401 means the token itself is bad (expired, revoked, security-stamp
 * mismatch) — log out and redirect to login. 403 means THIS action is denied
 * (OrderAccessDenied, ThisOwnerAlreadyHasAShop, NotEligibleToReview, ...) for
 * an otherwise-valid, still-logged-in user; logging them out on a 403 would be
 * wrong.
 */
export const authErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const tokenService = inject(AuthTokenService);

  return next(req).pipe(
    catchError((err) => {
      const status = err?.status ?? 0;
      if (status === 401) {
        try {
          tokenService.setToken(null);
          tokenService.setUserName(null);
        } catch {}

        const nextUrl = window?.location ? `${window.location.pathname}${window.location.search}` : '/';
        router.navigate(['/login'], { queryParams: { next: nextUrl } });
      }
      return throwError(() => err);
    })
  );
};
