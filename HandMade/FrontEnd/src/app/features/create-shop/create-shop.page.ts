import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';
import { ShopService } from '../../core/shop/shop.service';
import { FilesService } from '../../core/files/files.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { TPipe } from '../../core/i18n/t.pipe';
import { AuthTokenService } from '../../core/api/auth-token.service';
import { apiErrorMessage } from '../../core/api/api-error';

@Component({
  standalone: true,
  templateUrl: './create-shop.page.html',
  styleUrl: './create-shop.page.css',
  imports: [ReactiveFormsModule, NgIf, TPipe]
})
export class CreateShopPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly shop = inject(ShopService);
  private readonly files = inject(FilesService);
  private readonly router = inject(Router);
  private readonly tokenService = inject(AuthTokenService);
  readonly i18n = inject(I18nService);

  protected readonly loading = signal(false);
  protected readonly checkingShop = signal(true);
  protected readonly serverError = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);
  protected readonly lang = computed(() => this.i18n.lang());

  protected readonly form = this.fb.group({
    name: ['', Validators.required],
    description: [''],
    imageFile: [null as File | null, Validators.required]
  });

  ngOnInit(): void {
    if (!this.tokenService.token() || !this.tokenService.hasRole('Artist')) {
      this.router.navigate(['/']);
      return;
    }

    this.shop.getMyShop().subscribe({
      next: (shop) => {
        this.checkingShop.set(false);
        if (shop?.id) {
          this.router.navigate(['/my-shop']);
        }
      },
      error: () => this.checkingShop.set(false)
    });
  }

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.serverError.set(null);
    this.loading.set(true);
    const val = this.form.getRawValue();
    const file = val.imageFile as File;

    this.files.upload(file, 'Shop').subscribe({
      next: (uploadRes) => {
        this.shop.createShop({
          shopName: val.name!,
          description: val.description ?? '',
          imagePath: uploadRes.relativePath
        }).subscribe({
          next: () => {
            this.loading.set(false);
            const msg = this.lang() === 'ar' ? 'تم إنشاء المتجر بنجاح، في انتظار موافقة المشرف.' : 'Shop created successfully and sent for admin approval';
            this.successMessage.set(msg);
            setTimeout(() => this.router.navigate(['/']), 1600);
          },
          error: (err) => {
            this.loading.set(false);
            this.serverError.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل إنشاء المتجر.' : 'Failed to create shop.'));
          }
        });
      },
      error: (err) => {
        this.loading.set(false);
        this.serverError.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل رفع الصورة.' : 'Failed to upload image.'));
      }
    });
  }
}
