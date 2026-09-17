import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api/api.tokens';
import { ShopStatus } from '../api/api.models';
import { CreateShopRequest, MyShop, UpdateShopInfoRequest } from './shop.models';

/** The artist's own shop — shop identity only. Products live in ProductManagementService. */
@Injectable({ providedIn: 'root' })
export class ShopService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getMyShop(): Observable<MyShop> {
    return this.http.get<MyShop>(`${this.apiBaseUrl}/api/ShopManagement/myShop`);
  }

  createShop(request: CreateShopRequest): Observable<void> {
    return this.http.post<void>(`${this.apiBaseUrl}/api/ShopManagement`, request);
  }

  updateShop(request: UpdateShopInfoRequest): Observable<void> {
    return this.http.put<void>(`${this.apiBaseUrl}/api/ShopManagement`, request);
  }

  updateActivityStatus(shopId: string, status: Extract<ShopStatus, 'Active' | 'Inactive'>): Observable<void> {
    return this.http.patch<void>(`${this.apiBaseUrl}/api/ShopManagement/activity`, { shopId, status });
  }
}
