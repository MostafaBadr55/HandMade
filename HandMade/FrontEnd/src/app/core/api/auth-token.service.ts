import { Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'unlimited_token';
const USER_NAME_KEY = 'unlimited_user_name';

@Injectable({ providedIn: 'root' })
export class AuthTokenService {
  private readonly _token = signal<string | null>(null);
  private readonly _userName = signal<string | null>(null);

  constructor() {
    const stored = localStorage.getItem(STORAGE_KEY);
    this._token.set(stored ? stored : null);

    const storedName = localStorage.getItem(USER_NAME_KEY);
    this._userName.set(storedName ? storedName : null);
  }

  token(): string | null {
    return this._token();
  }

  userName(): string | null {
    return this._userName();
  }

  setToken(token: string | null): void {
    const normalized = token?.trim() ? token.trim() : null;
    this._token.set(normalized);

    if (normalized) {
      localStorage.setItem(STORAGE_KEY, normalized);
    } else {
      localStorage.removeItem(STORAGE_KEY);
    }
  }

  setUserName(name: string | null): void {
    const normalized = name?.trim() ? name.trim() : null;
    this._userName.set(normalized);

    if (normalized) {
      localStorage.setItem(USER_NAME_KEY, normalized);
    } else {
      localStorage.removeItem(USER_NAME_KEY);
    }
  }

  // Attempt to read role from stored JWT token payload
  role(): string | null {
    const roles = this.getRolesArray();
    return roles.length > 0 ? roles[0] : null;
  }

  hasRole(role: string): boolean {
    const roles = this.getRolesArray();
    return roles.some((r) => r.toLowerCase() === role.toLowerCase());
  }

  private getRolesArray(): string[] {
    const tok = this.token();
    if (!tok) return [];

    try {
      const parts = tok.split('.');
      if (parts.length < 2) return [];
      const payload = parts[1];
      // base64url -> base64
      const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
      const json = decodeURIComponent(
        atob(base64)
          .split('')
          .map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
          })
          .join('')
      );
      const obj = JSON.parse(json);
      
      let val = obj['role'] || obj['roles'];
      if (!val) {
        const key = Object.keys(obj).find((k) => k.toLowerCase().includes('/role'));
        if (key) val = obj[key];
      }
      
      if (!val) return [];
      if (Array.isArray(val)) return val.map((v: any) => String(v));
      return [String(val)];
    } catch (e) {
      return [];
    }
  }
}
