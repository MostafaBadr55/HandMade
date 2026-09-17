import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api/api.tokens';
import { ReviewTargetType } from '../api/api.models';

export interface CreateReviewRequest {
  targetType: Extract<ReviewTargetType, 'Product' | 'Shop'>;
  targetId: string;
  rating: number;
  title: string;
  content: string;
}

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  createReview(request: CreateReviewRequest): Observable<{ reviewId: string }> {
    return this.http.post<{ reviewId: string }>(`${this.apiBaseUrl}/api/Reviews`, request);
  }

  /** Eligibility requires a Delivered order against this product/shop. */
  checkEligibility(targetType: ReviewTargetType, targetId: string): Observable<{ canReview: boolean }> {
    const params = new HttpParams().set('targetType', targetType).set('targetId', targetId);
    return this.http.get<{ canReview: boolean }>(`${this.apiBaseUrl}/api/Reviews/eligibility`, { params });
  }
}
