import { Component, inject, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { StorefrontService } from '../../core/storefront/storefront.service';
import { CartService } from '../../core/cart/cart.service';
import { AuthTokenService } from '../../core/api/auth-token.service';
import { Category, HomePage, ProductCard, ShopCard } from '../../core/models/catalog.models';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, of } from 'rxjs';
import { I18nService } from '../../core/i18n/i18n.service';
import { TPipe } from '../../core/i18n/t.pipe';
import { apiErrorMessage } from '../../core/api/api-error';
import { resolveImageUrl } from '../../core/api/image-url';

import { ProductCardComponent } from '../../shared/components/product-card/product-card.component';

const EMPTY_HOME: HomePage = { categories: [], topRatedShops: [], mostRecentProducts: [] };

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, FormsModule, TPipe, RouterLink, ProductCardComponent],
  templateUrl: './landing.page.html',
  styleUrls: ['./landing.page.css']
})
export class LandingPage {
  private storefrontService = inject(StorefrontService);
  private cartService = inject(CartService);
  private tokenService = inject(AuthTokenService);
  private router = inject(Router);
  readonly i18n = inject(I18nService);
  lang = computed(() => this.i18n.lang());

  private home = toSignal(this.storefrontService.getHome().pipe(catchError(() => of(EMPTY_HOME))), {
    initialValue: EMPTY_HOME
  });

  categories = computed<Category[]>(() => this.home().categories);
  products = computed<ProductCard[]>(() => this.home().mostRecentProducts);
  topShops = computed<ShopCard[]>(() => this.home().topRatedShops);

  searchTerm = '';

  addingIds = signal<string[]>([]);

  resolveImage(url?: string | null) {
    return resolveImageUrl(url);
  }

  onSearch() {
    const term = this.searchTerm.trim();
    this.router.navigate(['/products'], term ? { queryParams: { searchTerm: term } } : {});
  }

  addToCart(product: ProductCard) {
    if (!this.tokenService.hasRole('Client')) {
      this.router.navigate(['/login'], { queryParams: { next: '/' } });
      return;
    }

    const id = product.productId;
    this.addingIds.update((arr) => [...arr, id]);
    this.cartService.addToCart(id, 1).subscribe({
      next: () => this.addingIds.update((arr) => arr.filter((x) => x !== id)),
      error: (err) => {
        this.addingIds.update((arr) => arr.filter((x) => x !== id));
        alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل إضافة المنتج للسلة' : 'Failed to add to cart'));
      }
    });
  }
}
