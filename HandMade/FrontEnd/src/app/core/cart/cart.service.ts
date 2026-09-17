import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal, computed } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { API_BASE_URL } from '../api/api.tokens';

export interface CartItem {
  cartItemId: string;
  productId: string;
  shopId: string;
  shopName: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  expectedDays: number;
  imageUrl?: string | null;
  isStillPurchasable: boolean;
}

export interface Cart {
  cartId: string;
  items: CartItem[];
  itemCount: number;
  subtotal: number;
}

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  // Signal to track cart items count for the header badge
  private readonly _cartCount = signal<number>(0);
  readonly cartCount = computed(() => this._cartCount());

  getCart(): Observable<Cart> {
    return this.http.get<Cart>(`${this.apiBaseUrl}/api/Cart`).pipe(
      tap((cart) => this._cartCount.set(cart.itemCount ?? cart.items?.length ?? 0))
    );
  }

  /** The API merges a duplicate product line itself — a single request suffices. */
  addToCart(productId: string, quantity: number = 1): Observable<void> {
    return this.http.post<void>(`${this.apiBaseUrl}/api/Cart/items`, { productId, quantity }).pipe(
      tap(() => this.refreshCartCount())
    );
  }

  updateItemQuantity(cartItemId: string, quantity: number): Observable<void> {
    return this.http.patch<void>(`${this.apiBaseUrl}/api/Cart/items/${cartItemId}`, { quantity });
  }

  removeItem(cartItemId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/api/Cart/items/${cartItemId}`).pipe(
      tap(() => this.refreshCartCount())
    );
  }

  clearCart(): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/api/Cart`).pipe(
      tap(() => this._cartCount.set(0))
    );
  }

  refreshCartCount(): void {
    this.getCart().subscribe({
      next: (cart) => this._cartCount.set(cart.itemCount ?? cart.items?.length ?? 0),
      error: () => this._cartCount.set(0)
    });
  }
}
