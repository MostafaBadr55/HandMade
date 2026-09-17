import { Component, inject, OnInit, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { StorefrontService } from '../../core/storefront/storefront.service';
import { CartService } from '../../core/cart/cart.service';
import { AuthTokenService } from '../../core/api/auth-token.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { apiErrorMessage } from '../../core/api/api-error';
import { ProductCard } from '../../core/models/catalog.models';

import { ProductCardComponent } from '../../shared/components/product-card/product-card.component';

@Component({
  standalone: true,
  imports: [CommonModule, ProductCardComponent],
  templateUrl: './category.page.html',
  styleUrls: ['./category.page.css']
})
export class CategoryPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly storefrontService = inject(StorefrontService);
  private readonly cartService = inject(CartService);
  private readonly tokenService = inject(AuthTokenService);
  readonly i18n = inject(I18nService);
  lang = computed(() => this.i18n.lang());

  loading = signal(false);
  products = signal<ProductCard[]>([]);
  categoryName = signal<string | null>(null);
  adding = signal<string[]>([]);
  private categoryId = '';
  pageNumber = signal(1);
  totalPages = signal(1);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('categoryId');
    if (!id) return;
    this.categoryId = id;

    this.storefrontService.getCategories().subscribe({
      next: (cats) => this.categoryName.set(cats.find((c) => c.id === id)?.name ?? null),
      error: () => {}
    });

    this.loadProducts();
  }

  loadProducts() {
    this.loading.set(true);
    this.storefrontService.getCategoryProducts(this.categoryId, this.pageNumber(), 20).subscribe({
      next: (res) => {
        this.products.set(res.items ?? []);
        this.totalPages.set(res.totalPages || 1);
        this.loading.set(false);
      },
      error: () => {
        this.products.set([]);
        this.loading.set(false);
      }
    });
  }

  nextPage() {
    this.pageNumber.update((p) => p + 1);
    this.loadProducts();
  }

  prevPage() {
    this.pageNumber.update((p) => Math.max(1, p - 1));
    this.loadProducts();
  }

  addToCart(product: ProductCard) {
    if (!this.tokenService.hasRole('Client')) {
      this.router.navigate(['/login']);
      return;
    }
    const id = product.productId;
    this.adding.update((arr) => [...arr, id]);
    this.cartService.addToCart(id, 1).subscribe({
      next: () => this.adding.update((arr) => arr.filter((x) => x !== id)),
      error: (err) => {
        this.adding.update((arr) => arr.filter((x) => x !== id));
        alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل إضافة المنتج للسلة' : 'Failed to add to cart'));
      }
    });
  }
}
