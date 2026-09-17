import { Component, inject, signal, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';
import { AuthTokenService } from '../../core/api/auth-token.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { TPipe } from '../../core/i18n/t.pipe';
import { apiErrorMessage } from '../../core/api/api-error';
import { AssignedRole } from '../../core/api/api.models';

@Component({
  standalone: true,
  templateUrl: './select-role.page.html',
  styleUrl: './select-role.page.css',
  imports: [RouterLink, NgIf, TPipe]
})
export class SelectRolePage {
  private readonly auth = inject(AuthService);
  private readonly tokenService = inject(AuthTokenService);
  private readonly router = inject(Router);
  readonly i18n = inject(I18nService);

  protected readonly loading = signal(false);
  protected readonly serverError = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);
  protected readonly lang = computed(() => this.i18n.lang());

  protected selectClient(): void {
    this.performSelection('Client');
  }

  protected selectArtist(): void {
    this.performSelection('Artist');
  }

  /**
   * select-role does not return a new token, and roles live only in the JWT —
   * so BOTH roles must re-login to pick up an updated token, not just Artist.
   */
  private performSelection(role: AssignedRole): void {
    const username = this.tokenService.userName();
    if (!username) {
      this.serverError.set(this.lang() === 'ar' ? 'تعذر تحديد اسم المستخدم، الرجاء تسجيل الدخول مرة أخرى.' : 'Could not determine your username — please log in again.');
      return;
    }

    this.serverError.set(null);
    this.loading.set(true);
    this.auth.selectRole({ username, selectedRole: role }).subscribe({
      next: () => {
        this.loading.set(false);
        const msg = this.lang() === 'ar'
          ? 'تم اختيار الدور. يرجى تسجيل الدخول مرة أخرى لإنهاء الإعداد.'
          : 'Role selected. Please log in again to finish setup.';
        this.successMessage.set(msg);

        this.tokenService.setToken(null);
        this.tokenService.setUserName(null);

        const next = role === 'Artist' ? '/create-shop' : '/';
        setTimeout(() => this.router.navigate(['/login'], { queryParams: { next } }), 1200);
      },
      error: (err) => {
        this.loading.set(false);
        this.serverError.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل اختيار الدور.' : 'Failed to select role.'));
      }
    });
  }
}
