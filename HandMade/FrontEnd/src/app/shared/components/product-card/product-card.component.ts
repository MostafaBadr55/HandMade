import { Component, Input, Output, EventEmitter, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { I18nService } from '../../../core/i18n/i18n.service';
import { resolveImageUrl } from '../../../core/api/image-url';

export type ProductCardMode = 'public' | 'seller' | 'admin';

/**
 * Accepts any of the three shapes the API returns for a product (public
 * ProductCard, artist SellerProduct, admin AdminProduct) — they disagree on
 * id/title/image field names, so this stays loosely typed and normalises in
 * the getters below rather than forcing one shape on every caller.
 */
@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.css']
})
export class ProductCardComponent {
  private readonly i18n = inject(I18nService);
  protected readonly lang = computed(() => this.i18n.lang());

  @Input() product: any;
  @Input() mode: ProductCardMode = 'public';
  @Input() addingToCart = false;

  get isSeller(): boolean {
    return this.mode === 'seller';
  }

  get isAdmin(): boolean {
    return this.mode === 'admin';
  }

  get isPublic(): boolean {
    return this.mode === 'public';
  }

  @Output() addToCart = new EventEmitter<any>();
  @Output() edit = new EventEmitter<any>();
  @Output() delete = new EventEmitter<any>();
  @Output() approve = new EventEmitter<any>();
  @Output() reject = new EventEmitter<any>();
  @Output() toggleStatus = new EventEmitter<any>();

  get productId(): string {
    return this.product?.id ?? this.product?.productId ?? '';
  }

  get imageUrl(): string {
    const rawUrl =
      this.product?.images?.[0]?.url ??
      this.product?.relativePath ??
      this.product?.imageUrl ??
      this.product?.mainImage;
    return resolveImageUrl(rawUrl);
  }

  get title(): string {
    return this.product?.title || this.product?.productName || this.product?.name || 'Untitled';
  }

  get price(): number {
    return this.product?.price ?? 0;
  }

  get status(): string {
    return this.product?.approvalStatus || this.product?.status || 'Active';
  }

  onAddToCart(event: Event) {
    event.stopPropagation();
    event.preventDefault();
    this.addToCart.emit(this.product);
  }

  onEdit(event: Event) {
    event.stopPropagation();
    event.preventDefault();
    this.edit.emit(this.product);
  }

  onDelete(event: Event) {
    event.stopPropagation();
    event.preventDefault();
    if (confirm(this.lang() === 'ar' ? 'هل أنت متأكد من حذف هذا المنتج؟' : 'Are you sure you want to delete this product?')) {
      this.delete.emit(this.product);
    }
  }

  onApprove(event: Event) {
    event.stopPropagation();
    event.preventDefault();
    this.approve.emit(this.product);
  }

  onReject(event: Event) {
    event.stopPropagation();
    event.preventDefault();
    this.reject.emit(this.product);
  }

  onToggleStatus(event: Event) {
    event.stopPropagation();
    event.preventDefault();
    this.toggleStatus.emit(this.product);
  }

  onImageError(ev: Event) {
    const img = ev.target as HTMLImageElement;
    if (img) img.src = 'assets/placeholder.png';
  }
}
