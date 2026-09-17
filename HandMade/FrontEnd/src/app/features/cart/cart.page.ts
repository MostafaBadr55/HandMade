import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CartService, Cart, CartItem } from '../../core/cart/cart.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { TPipe } from '../../core/i18n/t.pipe';
import { apiErrorMessage } from '../../core/api/api-error';
import { resolveImageUrl } from '../../core/api/image-url';

const EMPTY_CART: Cart = { cartId: '', items: [], itemCount: 0, subtotal: 0 };

@Component({
  standalone: true,
  templateUrl: './cart.page.html',
  styleUrl: './cart.page.css',
  imports: [CommonModule, RouterLink, TPipe]
})
export class CartPage implements OnInit {
  private readonly cartService = inject(CartService);
  private readonly router = inject(Router);
  readonly i18n = inject(I18nService);

  protected readonly loading = signal(true);
  protected readonly updating = signal<string | null>(null); // cartItemId being updated
  protected readonly error = signal<string | null>(null);
  protected readonly cart = signal<Cart>(EMPTY_CART);

  protected readonly lang = computed(() => this.i18n.lang());
  protected readonly items = computed(() => this.cart().items);
  protected readonly totalPrice = computed(() => this.cart().subtotal);
  protected readonly totalItems = computed(() => this.cart().itemCount);
  protected readonly isEmpty = computed(() => this.items().length === 0);
  protected readonly resolveImage = resolveImageUrl;

  ngOnInit(): void {
    this.loadCart();
  }

  protected loadCart(): void {
    this.loading.set(true);
    this.error.set(null);
    this.cartService.getCart().subscribe({
      next: (cart) => {
        this.cart.set(cart);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل تحميل السلة' : 'Failed to load cart'));
        this.loading.set(false);
      }
    });
  }

  incrementQuantity(item: CartItem): void {
    this.updateQuantity(item.cartItemId, item.quantity + 1);
  }

  decrementQuantity(item: CartItem): void {
    if (item.quantity <= 1) {
      this.removeItem(item.cartItemId);
      return;
    }
    this.updateQuantity(item.cartItemId, item.quantity - 1);
  }

  updateQuantity(cartItemId: string, quantity: number): void {
    this.updating.set(cartItemId);
    this.cartService.updateItemQuantity(cartItemId, quantity).subscribe({
      next: () => {
        this.updating.set(null);
        this.loadCart();
      },
      error: () => {
        this.updating.set(null);
        this.loadCart();
      }
    });
  }

  removeItem(cartItemId: string): void {
    this.updating.set(cartItemId);
    this.cartService.removeItem(cartItemId).subscribe({
      next: () => {
        this.updating.set(null);
        this.loadCart();
      },
      error: () => {
        this.updating.set(null);
        this.loadCart();
      }
    });
  }

  clearCart(): void {
    if (!confirm(this.lang() === 'ar' ? 'هل أنت متأكد من إفراغ السلة؟' : 'Are you sure you want to clear the cart?')) {
      return;
    }
    this.loading.set(true);
    this.cartService.clearCart().subscribe({
      next: () => {
        this.cart.set(EMPTY_CART);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.loadCart();
      }
    });
  }

  goToCheckout(): void {
    this.router.navigate(['/checkout']);
  }
}
