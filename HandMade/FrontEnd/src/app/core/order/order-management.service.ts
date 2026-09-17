import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api/api.tokens';
import { PagedResponse, OrderStatus, SortDirection } from '../api/api.models';
import { MyOrderSortBy } from './order.models';

export interface ShopOrderListItem {
  orderId: string;
  orderNumber: string;
  status: OrderStatus;
  buyerUserId: string;
  buyerUserName: string;
  productTitle: string;
  productImageUrl?: string | null;
  quantity: number;
  grandTotal: number;
  executionDays?: number | null;
  expectedDeliveryDate?: string | null;
  createdAt: string;
}

export interface ShopOrderDetails extends ShopOrderListItem {
  productId: string;
  unitPrice: number;
  subtotal: number;
  shippingFee: number;
  taxTotal: number;
  specialInstructions?: string | null;
  confirmedAt?: string | null;
  autoReleaseAt?: string | null;
  cancellationReason?: string | null;
  cancelledAt?: string | null;
  shippingAddressLabel?: string | null;
  shippingAddressDetails?: string | null;
  attachmentUrls: string[];
}

export interface GetShopOrdersQuery {
  status?: OrderStatus;
  sortBy?: MyOrderSortBy;
  sortDirection?: SortDirection;
  pageNumber?: number;
  pageSize?: number;
}

/** The artist half of the order negotiation. */
@Injectable({ providedIn: 'root' })
export class OrderManagementService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getShopOrders(query: GetShopOrdersQuery = {}): Observable<PagedResponse<ShopOrderListItem>> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query)) {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    }
    return this.http.get<PagedResponse<ShopOrderListItem>>(`${this.apiBaseUrl}/api/OrderManagement`, { params });
  }

  getOrder(orderId: string): Observable<ShopOrderDetails> {
    return this.http.get<ShopOrderDetails>(`${this.apiBaseUrl}/api/OrderManagement/${orderId}`);
  }

  /** SellerPending -> BuyerPending: the artist's price + timeline. */
  submitQuote(orderId: string, price: number, executionDays: number): Observable<void> {
    return this.http.patch<void>(`${this.apiBaseUrl}/api/OrderManagement/${orderId}/quote`, { price, executionDays });
  }

  /** SellerPending -> Cancelled: decline the request outright. */
  rejectRequest(orderId: string, reason?: string): Observable<void> {
    return this.http.patch<void>(`${this.apiBaseUrl}/api/OrderManagement/${orderId}/reject`, { reason });
  }

  /** InProgress -> CompletedBySeller: starts the escrow auto-release clock. */
  markComplete(orderId: string): Observable<void> {
    return this.http.patch<void>(`${this.apiBaseUrl}/api/OrderManagement/${orderId}/complete`, {});
  }
}
