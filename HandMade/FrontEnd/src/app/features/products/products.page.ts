import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { StorefrontService, PublicProductsQuery } from '../../core/storefront/storefront.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { CartService } from '../../core/cart/cart.service';
import { AuthTokenService } from '../../core/api/auth-token.service';
import { apiErrorMessage } from '../../core/api/api-error';
import { Category, ProductCard, SubCategory } from '../../core/models/catalog.models';

import { ProductCardComponent } from '../../shared/components/product-card/product-card.component';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, ProductCardComponent],
  templateUrl: './products.page.html',
  styleUrls: ['./products.page.css']
})
export class ProductsPage implements OnInit {
  private readonly storefrontService = inject(StorefrontService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly cartService = inject(CartService);
  private readonly tokenService = inject(AuthTokenService);
  readonly i18n = inject(I18nService);
  lang = computed(() => this.i18n.lang());

  categoryName = signal<string | null>(null);

  loading = signal(false);
  products = signal<ProductCard[]>([]);
  categories = signal<Category[]>([]);
  subCategories = signal<SubCategory[]>([]);
  totalPages = signal(1);
  adding = signal<string[]>([]);

  filters = signal<PublicProductsQuery>({
    categoryId: undefined,
    subCategoryId: undefined,
    searchTerm: '',
    sortBy: undefined,
    sortDirection: undefined,
    pageNumber: 1,
    pageSize: 20
  });

  ngOnInit() {
    const qp = this.route.snapshot.queryParamMap;
    const categoryId = qp.get('categoryId') ?? undefined;
    const subCategoryId = qp.get('subCategoryId') ?? undefined;
    const searchTerm = qp.get('searchTerm') ?? undefined;

    this.filters.update((f) => ({ ...f, categoryId, subCategoryId, searchTerm: searchTerm ?? '' }));

    this.storefrontService.getCategories().subscribe({
      next: (cats) => {
        this.categories.set(cats || []);
        if (categoryId) {
          this.applyRouteSubCategories(categoryId);
          this.categoryName.set(cats.find((c) => c.id === categoryId)?.name ?? null);
        }
      },
      error: () => {}
    });

    this.loadProducts();
  }

  private applyRouteSubCategories(categoryId: string) {
    const cat = this.categories().find((c) => c.id === categoryId);
    this.subCategories.set(cat?.subCategories ?? []);
  }

  loadProducts() {
    this.loading.set(true);
    this.storefrontService.getProducts(this.filters()).subscribe({
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

  addToCart(product: ProductCard) {
    if (!this.tokenService.hasRole('Client')) {
      this.router.navigate(['/login'], { queryParams: { next: '/products' } });
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

  updateCategory(categoryId: string) {
    this.filters.update((f) => ({ ...f, categoryId: categoryId || undefined, subCategoryId: undefined, pageNumber: 1 }));
    this.applyRouteSubCategories(categoryId);
    this.loadProducts();
  }

  updateSubCategory(subCategoryId: string) {
    this.filters.update((f) => ({ ...f, subCategoryId: subCategoryId || undefined, pageNumber: 1 }));
    this.loadProducts();
  }

  updateSearch(value: string) {
    this.filters.update((f) => ({ ...f, searchTerm: value }));
  }

  updateSortBy(value: string) {
    this.filters.update((f) => ({ ...f, sortBy: (value || undefined) as 'Price' | 'CreatedAt' | undefined, pageNumber: 1 }));
    this.loadProducts();
  }

  updateSortDirection(value: string) {
    this.filters.update((f) => ({ ...f, sortDirection: (value || undefined) as 'Asc' | 'Desc' | undefined, pageNumber: 1 }));
    this.loadProducts();
  }

  applyFilters() {
    this.filters.update((f) => ({ ...f, pageNumber: 1 }));
    this.loadProducts();
  }

  resetFilters() {
    this.categoryName.set(null);
    this.subCategories.set([]);
    this.filters.set({
      categoryId: undefined,
      subCategoryId: undefined,
      searchTerm: '',
      sortBy: undefined,
      sortDirection: undefined,
      pageNumber: 1,
      pageSize: 20
    });
    this.loadProducts();
  }

  nextPage() {
    this.filters.update((f) => ({ ...f, pageNumber: (f.pageNumber ?? 1) + 1 }));
    this.loadProducts();
  }

  prevPage() {
    this.filters.update((f) => ({ ...f, pageNumber: Math.max(1, (f.pageNumber ?? 1) - 1) }));
    this.loadProducts();
  }
}
