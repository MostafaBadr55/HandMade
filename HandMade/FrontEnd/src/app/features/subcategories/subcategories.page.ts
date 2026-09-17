import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { map, switchMap } from 'rxjs';

import { StorefrontService } from '../../core/storefront/storefront.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { TPipe } from '../../core/i18n/t.pipe';
import { resolveImageUrl } from '../../core/api/image-url';

@Component({
  standalone: true,
  templateUrl: './subcategories.page.html',
  styleUrl: './subcategories.page.css',
  imports: [AsyncPipe, NgIf, NgFor, RouterLink, TPipe]
})
export class SubCategoriesPage {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly storefrontService = inject(StorefrontService);
  private readonly i18n = inject(I18nService);

  protected readonly lang = computed(() => this.i18n.lang());
  protected readonly resolveImage = resolveImageUrl;

  private readonly categoryId$ = this.route.paramMap.pipe(map((p) => p.get('categoryId')!));

  protected readonly vm$ = this.categoryId$.pipe(
    switchMap((categoryId) =>
      this.storefrontService.getCategories().pipe(
        map((categories) => ({
          categoryId,
          category: categories.find((c) => c.id === categoryId) ?? null,
          subCategories: categories.find((c) => c.id === categoryId)?.subCategories ?? []
        }))
      )
    )
  );

  protected back(): void {
    this.router.navigate(['/']);
  }
}
