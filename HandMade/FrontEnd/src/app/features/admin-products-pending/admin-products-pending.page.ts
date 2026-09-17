import { Component, inject, signal, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../core/admin/admin.service';
import { AdminProduct } from '../../core/admin/admin.models';
import { I18nService } from '../../core/i18n/i18n.service';
import { apiErrorMessage } from '../../core/api/api-error';

import { ProductCardComponent } from '../../shared/components/product-card/product-card.component';

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink, ProductCardComponent],
  templateUrl: './admin-products-pending.page.html',
  styleUrls: ['./admin-products-pending.page.css']
})
export class AdminProductsPendingPage {
  private admin = inject(AdminService);
  readonly i18n = inject(I18nService);
  lang = computed(() => this.i18n.lang());
  protected readonly loading = signal(true);
  protected readonly products = signal<AdminProduct[]>([]);

  constructor() {
    this.loadPending();
  }

  loadPending() {
    this.loading.set(true);
    this.admin.getProducts({ approvalStatus: 'Pending', pageSize: 100 }).subscribe({
      next: (res) => {
        this.products.set(res.items ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.products.set([]);
        this.loading.set(false);
      }
    });
  }

  approve(product: AdminProduct) {
    this.admin.approveProduct(product.id).subscribe({
      next: () => this.products.set(this.products().filter((p) => p.id !== product.id)),
      error: (err) => alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل الموافقة على المنتج.' : 'Failed to approve product.'))
    });
  }

  reject(product: AdminProduct) {
    const msg = prompt(this.lang() === 'ar' ? 'سبب الرفض (اختياري)' : 'Rejection reason (optional)');
    this.admin.rejectProduct(product.id, msg || '').subscribe({
      next: () => this.products.set(this.products().filter((p) => p.id !== product.id)),
      error: (err) => alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل رفض المنتج.' : 'Failed to reject product.'))
    });
  }
}
