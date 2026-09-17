import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../api/api.tokens';
import { PagedResponse, ReviewTargetType } from '../api/api.models';
import { Category, HomePage, ProductCard, ProductDetails, ProductReview, ReviewSummary } from '../models/catalog.models';

export interface PublicProductsQuery {
  shopId?: string;
  categoryId?: string;
  subCategoryId?: string;
  searchTerm?: string;
  sortBy?: 'Price' | 'CreatedAt';
  sortDirection?: 'Asc' | 'Desc';
  pageNumber?: number;
  pageSize?: number;
}

/** Public, anonymous reads against /api/StoreFront/*. */
@Injectable({ providedIn: 'root' })
export class StorefrontService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  getHome(): Observable<HomePage> {
    return this.http.get<HomePage>(`${this.baseUrl}/api/StoreFront/home`);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}/api/StoreFront/categories`);
  }

  getProducts(query: PublicProductsQuery): Observable<PagedResponse<ProductCard>> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query)) {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    }
    return this.http.get<PagedResponse<ProductCard>>(`${this.baseUrl}/api/StoreFront/products`, { params });
  }

  getCategoryProducts(categoryId: string, pageNumber = 1, pageSize = 20): Observable<PagedResponse<ProductCard>> {
    const params = new HttpParams()
      .set('categoryId', categoryId)
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);
    return this.http.get<PagedResponse<ProductCard>>(`${this.baseUrl}/api/StoreFront/CategoryProducts`, { params });
  }

  getShopProducts(shopId: string, pageNumber = 1, pageSize = 20): Observable<PagedResponse<ProductCard>> {
    const params = new HttpParams()
      .set('shopId', shopId)
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);
    return this.http.get<PagedResponse<ProductCard>>(`${this.baseUrl}/api/StoreFront/ShopProducts`, { params });
  }

  getProductDetails(productId: string): Observable<ProductDetails> {
    return this.http.get<ProductDetails>(`${this.baseUrl}/api/StoreFront/products/${productId}`);
  }

  getReviews(
    targetType: ReviewTargetType,
    targetId: string,
    pageNumber = 1,
    pageSize = 20
  ): Observable<PagedResponse<ProductReview>> {
    const params = new HttpParams()
      .set('targetType', targetType)
      .set('targetId', targetId)
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);
    return this.http.get<PagedResponse<ProductReview>>(`${this.baseUrl}/api/StoreFront/reviews`, { params });
  }

  getReviewSummary(targetType: ReviewTargetType, targetId: string): Observable<ReviewSummary> {
    const params = new HttpParams().set('targetType', targetType).set('targetId', targetId);
    return this.http.get<ReviewSummary>(`${this.baseUrl}/api/StoreFront/reviews/summary`, { params });
  }
}
