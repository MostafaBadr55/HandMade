import { ShopStatus } from '../api/api.models';
import { SellerProduct } from '../models/catalog.models';

export interface MyShop {
  id: string;
  ownerUserName: string;
  name: string;
  imageUrl: string;
  description: string;
  status: ShopStatus;
  ratingAverage: number;
  rejectionMessage?: string | null;
  activeProductDtos: SellerProduct[];
  inActiveProductDtos: SellerProduct[];
}

export interface CreateShopRequest {
  shopName: string;
  description?: string;
  imagePath: string;
}

export interface UpdateShopInfoRequest {
  shopId: string;
  shopName: string;
  shopDescription: string;
  imageRelativePath: string;
}
