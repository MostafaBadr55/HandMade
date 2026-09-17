import { Component, inject, computed } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { NgIf, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StorefrontService } from '../../../core/storefront/storefront.service';
import { Category } from '../../../core/models/catalog.models';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, of } from 'rxjs';
import { AuthTokenService } from '../../../core/api/auth-token.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { TPipe } from '../../../core/i18n/t.pipe';
import { CartService } from '../../../core/cart/cart.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, NgIf, CommonModule, FormsModule, TPipe],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  private storefrontService = inject(StorefrontService);
  private router = inject(Router);
  private tokenService = inject(AuthTokenService);
  private cartService = inject(CartService);
  readonly i18n = inject(I18nService);

  isLoggedIn = computed(() => !!this.tokenService.token());
  userName = computed(() => this.tokenService.userName() || 'User');
  lang = computed(() => this.i18n.lang());
  cartCount = computed(() => this.cartService.cartCount());

  isAdmin = computed(() => this.tokenService.hasRole('Admin') || this.tokenService.hasRole('SuperAdmin'));
  isArtist = computed(() => this.tokenService.hasRole('Artist'));
  isClient = computed(() => this.tokenService.hasRole('Client'));

  profileLink = computed(() => {
    if (this.isAdmin()) return '/admin';
    if (this.isArtist()) return '/my-shop';
    return '/addresses';
  });

  categories = toSignal(this.storefrontService.getCategories().pipe(
    catchError(() => of([]))
  ), { initialValue: [] as Category[] });

  searchTerm = '';

  onSearch() {
    const term = this.searchTerm.trim();
    this.router.navigate(['/products'], term ? { queryParams: { searchTerm: term } } : {});
  }

  hideTopBar() {
    const url = this.router.url || '';
    return url.includes('/login') || url.includes('/register');
  }

  constructor() {
    if (this.isLoggedIn()) {
      this.cartService.refreshCartCount();
    }
  }

  toggleLang() {
    const newLang = this.i18n.lang() === 'en' ? 'ar' : 'en';
    this.i18n.setLang(newLang);
  }

  logout() {
    this.tokenService.setToken(null);
    this.tokenService.setUserName(null);
    this.router.navigate(['/login']);
  }
}
