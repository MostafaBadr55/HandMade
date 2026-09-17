import { ProductApprovalStatus, ProductStatus } from '../api/api.models';

export interface SubCategory {
  id: string;
  categoryId: string;
  name: string;
}

export interface Category {
  id: string;
  name: string;
  description?: string;
  imageUrl?: string;
  subCategories?: SubCategory[];
}

export interface ProductCard {
  productId: string;
  shopId: string;
  productName: string;
  shopName: string;
  price: number;
  expectedDays: number;
  averageRating: number | null;
  reviewCount: number;
  relativePath?: string | null;
  altText?: string | null;
}

export interface ProductImage {
  id: string;
  url: string;
  altText?: string;
  isPrimary: boolean;
  sortOrder: number;
}

export interface ProductReview {
  reviewerName: string;
  reviewTitle: string;
  reviewContent: string;
  rating: number;
}

export interface ProductDetails {
  productId: string;
  shopId: string;
  shopName: string;
  productName: string;
  averageRating: number | null;
  reviewCount: number;
  price: number;
  expectedDays: number;
  description: string;
  reviews: ProductReview[];
  images: ProductImage[];
}

export interface ShopCard {
  shopId: string;
  shopName: string;
  description: string;
  mainImage: string;
  rating: number;
}

export interface HomePage {
  categories: Category[];
  topRatedShops: ShopCard[];
  mostRecentProducts: ProductCard[];
}

export interface ReviewSummary {
  averageRating: number | null;
  reviewCount: number;
}

// Artist-facing product shape (ProductForSellerDTO), used on my-shop / product management.
export interface SellerProduct {
  id: string;
  title: string;
  description?: string;
  price: number;
  status: ProductStatus;
  approvalStatus?: ProductApprovalStatus;
  isPublished: boolean;
  images: SellerProductImage[];
}

export interface SellerProductImage {
  id: string;
  productId: string;
  url: string;
  altText: string;
  sortOrder: number;
  isPrimary: boolean;
}
