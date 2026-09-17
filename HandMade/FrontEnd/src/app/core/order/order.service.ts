import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api/api.tokens';
import { PagedResponse } from '../api/api.models';
import {
  AcceptOrderQuoteRequest,
  CheckoutCartRequest,
  GetMyOrdersQuery,
  OrderDetails,
  OrderListItem,
  PlaceOrderRequest
} from './order.models';

/** The buyer half of the order negotiation. */
@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getMyOrders(query: GetMyOrdersQuery = {}): Observable<PagedResponse<OrderListItem>> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query)) {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    }
    return this.http.get<PagedResponse<OrderListItem>>(`${this.apiBaseUrl}/api/Orders`, { params });
  }

  getOrder(orderId: string): Observable<OrderDetails> {
    return this.http.get<OrderDetails>(`${this.apiBaseUrl}/api/Orders/${orderId}`);
  }

  /** Request a quote for a single product — the negotiation entry point. */
  placeOrderRequest(request: PlaceOrderRequest): Observable<{ orderId: string }> {
    return this.http.post<{ orderId: string }>(`${this.apiBaseUrl}/api/Orders/requests`, request);
  }

  /** Converts every live cart line into its own negotiation, in one transaction. */
  checkout(request: CheckoutCartRequest): Observable<{ orderIds: string[] }> {
    return this.http.post<{ orderIds: string[] }>(`${this.apiBaseUrl}/api/Orders/checkout`, request);
  }

  /** Accepts the artist's quote and charges the client — this is the escrow charge. */
  acceptQuote(orderId: string, method: AcceptOrderQuoteRequest['method'] = 'CreditCard'): Observable<void> {
    return this.http.patch<void>(`${this.apiBaseUrl}/api/Orders/${orderId}/accept`, { method });
  }

  rejectQuote(orderId: string, reason?: string): Observable<void> {
    return this.http.patch<void>(`${this.apiBaseUrl}/api/Orders/${orderId}/reject`, { reason: reason ?? '' });
  }

  cancelOrder(orderId: string, reason?: string): Observable<void> {
    return this.http.patch<void>(`${this.apiBaseUrl}/api/Orders/${orderId}/cancel`, { reason: reason ?? '' });
  }

  /** Confirms receipt — this is what releases the escrowed funds to the artist. */
  confirmDelivery(orderId: string): Observable<void> {
    return this.http.patch<void>(`${this.apiBaseUrl}/api/Orders/${orderId}/confirm-delivery`, {});
  }
}
