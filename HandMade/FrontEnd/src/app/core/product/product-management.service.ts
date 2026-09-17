import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api/api.tokens';
import { PagedResponse, ProductApprovalStatus, ProductStatus, SortDirection } from '../api/api.models';
import { SellerProduct } from '../models/catalog.models';

export interface ProductImagePayload {
  relativePath: string;
  altText?: string;
  isPrimary: boolean;
  sortOrder: number;
}

export interface CreateProductRequest {
  shopId: string;
  categoryId: string;
  subCategoryId: string;
  title: string;
  description: string;
  price: number;
  images: ProductImagePayload[];
}

export interface UpdateProductImagePayload {
  url: string; // relative path, matched against stored rows
  altText?: string;
  isPrimary: boolean;
  sortOrder: number;
}

export interface UpdateProductMainInfoRequest {
  title: string;
  categoryId: string;
  subCategoryId: string;
  images: UpdateProductImagePayload[];
}

export type ProductSortBy = 'CreatedAt' | 'Price' | 'Title';

export interface GetShopProductsQuery {
  status?: ProductStatus;
  approvalStatus?: ProductApprovalStatus;
  isPublished?: boolean;
  sortBy?: ProductSortBy;
  sortDirection?: SortDirection;
  pageNumber?: number;
  pageSize?: number;
}

/** The artist's own product CRUD, scoped to one shop at a time. */
@Injectable({ providedIn: 'root' })
export class ProductManagementService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getShopProducts(shopId: string, query: GetShopProductsQuery = {}): Observable<PagedResponse<SellerProduct>> {
    let params = new HttpParams().set('shopId', shopId);
    for (const [key, value] of Object.entries(query)) {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    }
    return this.http.get<PagedResponse<SellerProduct>>(`${this.apiBaseUrl}/api/ProductManagement`, { params });
  }

  createProduct(request: CreateProductRequest): Observable<{ message: string; id: string }> {
    return this.http.post<{ message: string; id: string }>(`${this.apiBaseUrl}/api/ProductManagement`, request);
  }

  updateProductMainInfo(productId: string, shopId: string, request: UpdateProductMainInfoRequest): Observable<void> {
    const params = new HttpParams().set('shopId', shopId);
    return this.http.put<void>(`${this.apiBaseUrl}/api/ProductManagement/${productId}`, request, { params });
  }

  updateProductPrice(productId: string, shopId: string, price: number): Observable<void> {
    const params = new HttpParams().set('shopId', shopId);
    return this.http.patch<void>(`${this.apiBaseUrl}/api/ProductManagement/${productId}/price`, { price }, { params });
  }

  updateProductStatus(productId: string, shopId: string, status: ProductStatus): Observable<void> {
    const params = new HttpParams().set('shopId', shopId);
    return this.http.patch<void>(`${this.apiBaseUrl}/api/ProductManagement/${productId}/status`, { status }, { params });
  }

  deleteProduct(productId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/api/ProductManagement/${productId}`);
  }
}
