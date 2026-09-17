import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthTokenService } from '../../../core/api/auth-token.service';
import { StorefrontService } from '../../../core/storefront/storefront.service';
import { Category, SubCategory } from '../../../core/models/catalog.models';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, of } from 'rxjs';
import { I18nService } from '../../../core/i18n/i18n.service';
import { TPipe } from '../../../core/i18n/t.pipe';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, TPipe],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  private readonly tokenService = inject(AuthTokenService);
  private readonly storefrontService = inject(StorefrontService);
  private readonly router = inject(Router);
  readonly i18n = inject(I18nService);

  isLoggedIn = computed(() => !!this.tokenService.token());
  isArtist = computed(() => this.tokenService.hasRole('Artist'));
  isClient = computed(() => this.tokenService.hasRole('Client'));
  isAdmin = computed(() => this.tokenService.hasRole('Admin') || this.tokenService.hasRole('SuperAdmin'));
  lang = computed(() => this.i18n.lang());

  profileLink = computed(() => {
    if (this.isAdmin()) return '/admin';
    if (this.isArtist()) return '/my-shop';
    return '/addresses';
  });

  categories = toSignal(this.storefrontService.getCategories().pipe(
    catchError(() => of([]))
  ), { initialValue: [] as Category[] });

  showMenu = signal(false);
  expandedCategoryId = signal<string | null>(null);

  currentSubCategories = computed<SubCategory[]>(() => {
    const id = this.expandedCategoryId();
    if (!id) return [];
    return this.categories().find((c) => c.id === id)?.subCategories ?? [];
  });

  toggleMenu() {
    this.showMenu.update((v) => !v);
    if (!this.showMenu()) {
      this.expandedCategoryId.set(null);
    }
  }

  toggleLang() {
    const newLang = this.i18n.lang() === 'en' ? 'ar' : 'en';
    this.i18n.setLang(newLang);
  }

  toggleCategory(categoryId: string) {
    this.expandedCategoryId.set(this.expandedCategoryId() === categoryId ? null : categoryId);
  }

  logout() {
    this.tokenService.setToken(null);
    this.tokenService.setUserName(null);
    this.showMenu.set(false);
    this.router.navigate(['/login']);
  }
}
