import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { API_BASE_URL } from '../api/api.tokens';
import { AuthTokenService } from '../api/auth-token.service';
import { AuthResponse, LoginRequest, RegisterRequest, SelectRoleRequest, SelectRoleResponse } from './auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);
  private readonly tokenService = inject(AuthTokenService);

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiBaseUrl}/api/Account/register`, data);
  }

  /**
   * Registration assigns no role, so the token from register() carries zero
   * role claims. login() stores the token + username so the app can proceed.
   */
  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiBaseUrl}/api/Account/login`, data).pipe(
      tap((response) => {
        this.tokenService.setToken(response.token);
        this.tokenService.setUserName(data.username);
      })
    );
  }

  /**
   * select-role does NOT return a new token, and roles live only inside the
   * JWT — the caller must force a fresh login after this succeeds, or every
   * [Authorize(Roles=...)] call will 403 against the stale, roleless token.
   */
  selectRole(payload: SelectRoleRequest): Observable<SelectRoleResponse> {
    return this.http.post<SelectRoleResponse>(`${this.apiBaseUrl}/api/Account/users/select-role`, payload);
  }
}
