import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api/api.tokens';
import { PagedResponse } from '../api/api.models';
import { Category, SubCategory } from '../models/catalog.models';
import { AdminProduct, AdminShop, GetAdminProductsParams, GetShopsParams } from './admin.models';

function toHttpParams<T extends object>(obj: T): HttpParams {
  let params = new HttpParams();
  for (const [key, value] of Object.entries(obj)) {
    if (value !== null && value !== undefined && value !== '') {
      params = params.set(key, String(value));
    }
  }
  return params;
}

/** Admin dashboard reads/writes against /api/AdminDashboard/*. */
@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  // --- Shops ---

  getShops(params: GetShopsParams): Observable<PagedResponse<AdminShop>> {
    return this.http.get<PagedResponse<AdminShop>>(`${this.apiBaseUrl}/api/AdminDashboard/shops`, {
      params: toHttpParams(params)
    });
  }

  approveShop(shopId: string): Observable<{ message: string }> {
    return this.http.patch<{ message: string }>(`${this.apiBaseUrl}/api/AdminDashboard/shops/${shopId}/approve`, null);
  }

  rejectShop(shopId: string, rejectionMessage: string): Observable<{ message: string }> {
    return this.http.patch<{ message: string }>(`${this.apiBaseUrl}/api/AdminDashboard/shops/${shopId}/reject`, {
      shopId,
      rejectionMessage
    });
  }

  // --- Products ---

  getProducts(params: GetAdminProductsParams): Observable<PagedResponse<AdminProduct>> {
    return this.http.get<PagedResponse<AdminProduct>>(`${this.apiBaseUrl}/api/AdminDashboard/products`, {
      params: toHttpParams(params)
    });
  }

  approveProduct(productId: string): Observable<{ message: string }> {
    return this.http.patch<{ message: string }>(`${this.apiBaseUrl}/api/AdminDashboard/products/${productId}/approve`, null);
  }

  rejectProduct(productId: string, rejectionMessage: string): Observable<{ message: string }> {
    return this.http.patch<{ message: string }>(`${this.apiBaseUrl}/api/AdminDashboard/products/${productId}/reject`, {
      productId,
      rejectionMessage
    });
  }

  // --- Categories ---

  getCategories(searchTerm?: string, pageNumber = 1, pageSize = 100): Observable<PagedResponse<Category>> {
    return this.http.get<PagedResponse<Category>>(`${this.apiBaseUrl}/api/AdminDashboard/categories`, {
      params: toHttpParams({ searchTerm, pageNumber, pageSize })
    });
  }

  createCategory(payload: { name: string; description?: string; imageUrl?: string }): Observable<string> {
    return this.http.post<string>(`${this.apiBaseUrl}/api/AdminDashboard/categories`, payload);
  }

  updateCategory(id: string, payload: { name: string; description?: string; imageUrl?: string }): Observable<void> {
    return this.http.put<void>(`${this.apiBaseUrl}/api/AdminDashboard/categories/${id}`, payload);
  }

  deleteCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/api/AdminDashboard/categories/${id}`);
  }

  // --- Subcategories ---

  createSubCategory(payload: { categoryId: string; name: string }): Observable<string> {
    return this.http.post<string>(`${this.apiBaseUrl}/api/AdminDashboard/subcategories`, payload);
  }

  updateSubCategory(id: string, payload: { categoryId: string; name: string }): Observable<void> {
    return this.http.put<void>(`${this.apiBaseUrl}/api/AdminDashboard/subcategories/${id}`, payload);
  }

  deleteSubCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/api/AdminDashboard/subcategories/${id}`);
  }
}
