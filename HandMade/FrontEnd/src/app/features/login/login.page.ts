import { Component, inject, signal, computed } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { AuthTokenService } from '../../core/api/auth-token.service';
import { NgIf } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';
import { ShopService } from '../../core/shop/shop.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { TPipe } from '../../core/i18n/t.pipe';
import { apiErrorMessage } from '../../core/api/api-error';

@Component({
  standalone: true,
  templateUrl: './login.page.html',
  styleUrl: './login.page.css',
  imports: [ReactiveFormsModule, RouterLink, NgIf, TPipe]
})
export class LoginPage {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly tokenService = inject(AuthTokenService);
  private readonly shopService = inject(ShopService);
  readonly i18n = inject(I18nService);

  protected readonly loading = signal(false);
  protected readonly serverError = signal<string | null>(null);
  protected readonly lang = computed(() => this.i18n.lang());

  // Format validation belongs on register, not login — an existing account's
  // password may predate the current policy and must still be accepted here.
  protected readonly form = this.fb.group({
    userName: ['', Validators.required],
    password: ['', Validators.required]
  });

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.serverError.set(null);
    this.loading.set(true);
    const { userName, password } = this.form.getRawValue();

    this.auth.login({ username: userName!, password: password! }).subscribe({
      next: () => {
        this.loading.set(false);
        const role = this.tokenService.role();
        const nextPath = this.route.snapshot.queryParamMap.get('next');
        if (!role) {
          this.router.navigate(['/select-role']);
          return;
        }

        // Artists: route to /my-shop when their shop still needs attention.
        if (this.tokenService.hasRole('Artist')) {
          this.shopService.getMyShop().subscribe({
            next: (s) => {
              const status = (s?.status || '').toString().toLowerCase();
              if (status === 'pending' || status === 'rejected') {
                this.router.navigate(['/my-shop']);
              } else if (nextPath) {
                this.router.navigateByUrl(nextPath);
              } else {
                this.router.navigate(['/']);
              }
            },
            error: () => {
              if (nextPath) this.router.navigateByUrl(nextPath);
              else this.router.navigate(['/']);
            }
          });
          return;
        }

        if (nextPath) this.router.navigateByUrl(nextPath);
        else this.router.navigate(['/']);
      },
      error: (err) => {
        this.loading.set(false);
        this.serverError.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل تسجيل الدخول، تأكد من البيانات.' : 'Login failed. Please check your details.'));
      }
    });
  }
}
