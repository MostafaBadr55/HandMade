import { Injectable, inject } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router, UrlTree } from '@angular/router';
import { AuthTokenService } from '../api/auth-token.service';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
  private readonly tokenService = inject(AuthTokenService);
  private readonly router = inject(Router);

  canActivate(route: ActivatedRouteSnapshot): boolean | UrlTree {
    const token = this.tokenService.token();

    // If no token at all, user must login first
    if (!token) {
      return this.router.parseUrl('/login');
    }

    const allowed: string[] = route.data?.['roles'] ?? [];
    const role = this.tokenService.role();

    // If logged in but no role assigned, send to select-role
    if (!role) {
      return this.router.parseUrl('/select-role');
    }

    if (allowed.length === 0) return true;

    // An Artist also holds the Client role (the backend grants both), so the
    // token carries a role ARRAY — check membership across all of them, not
    // just the first.
    const ok = allowed.some((r) => this.tokenService.hasRole(r));
    return ok ? true : this.router.parseUrl('/');
  }
}

// Guard for select-role page - only show if logged in but no role
@Injectable({ providedIn: 'root' })
export class SelectRoleGuard implements CanActivate {
  private readonly tokenService = inject(AuthTokenService);
  private readonly router = inject(Router);

  canActivate(): boolean | UrlTree {
    const token = this.tokenService.token();

    // If no token, must login first
    if (!token) {
      return this.router.parseUrl('/login');
    }

    const role = this.tokenService.role();

    // If already has a role, redirect to home
    if (role) {
      return this.router.parseUrl('/');
    }

    // Has token but no role - can access select-role
    return true;
  }
}
