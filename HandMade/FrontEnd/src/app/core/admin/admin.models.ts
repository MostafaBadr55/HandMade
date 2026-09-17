import { ProductApprovalStatus, ProductStatus, ShopStatus, SortDirection } from '../api/api.models';

export type ShopSortBy = 'CreatedAt' | 'Name' | 'Rating';
export type ProductSortBy = 'CreatedAt' | 'Price' | 'Title';

export interface GetShopsParams {
  ownerUserId?: string;
  status?: ShopStatus;
  minRating?: number;
  maxRating?: number;
  name?: string;
  sortBy?: ShopSortBy;
  sortDirection?: SortDirection;
  pageNumber?: number;
  pageSize?: number;
}

export interface AdminShop {
  id: string;
  ownerUserName: string;
  name: string;
  imageUrl: string;
  description: string;
  status: ShopStatus;
  createdAt: string;
  ratingAverage: number;
}

export interface GetAdminProductsParams {
  shopId?: string;
  status?: ProductStatus;
  approvalStatus?: ProductApprovalStatus;
  isPublished?: boolean;
  sortBy?: ProductSortBy;
  sortDirection?: SortDirection;
  pageNumber?: number;
  pageSize?: number;
}

export interface AdminProduct {
  id: string;
  shopName?: string;
  isPublished?: boolean;
  title?: string;
  price?: number;
  status?: ProductStatus;
  images: { id: string; productId: string; url: string; altText: string; sortOrder: number; isPrimary: boolean }[];
}
