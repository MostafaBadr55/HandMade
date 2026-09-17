import { EscrowStatus, OrderStatus, PaymentMethod, PaymentStatus } from '../api/api.models';

export interface OrderListItem {
  orderId: string;
  orderNumber: string;
  status: OrderStatus;
  shopId: string;
  shopName: string;
  productTitle: string;
  productImageUrl?: string | null;
  quantity: number;
  grandTotal: number;
  executionDays?: number | null;
  expectedDeliveryDate?: string | null;
  createdAt: string;
}

export interface OrderDetails {
  orderId: string;
  orderNumber: string;
  status: OrderStatus;
  shopId: string;
  shopName: string;
  productId: string;
  productTitle: string;
  productImageUrl?: string | null;
  quantity: number;
  unitPrice: number;
  subtotal: number;
  shippingFee: number;
  taxTotal: number;
  grandTotal: number;
  executionDays?: number | null;
  specialInstructions?: string | null;
  confirmedAt?: string | null;
  expectedDeliveryDate?: string | null;
  autoReleaseAt?: string | null;
  cancellationReason?: string | null;
  cancelledAt?: string | null;
  shippingAddressLabel?: string | null;
  shippingAddressDetails?: string | null;
  paymentStatus?: PaymentStatus | null;
  escrowStatus?: EscrowStatus | null;
  attachmentUrls: string[];
  createdAt: string;
}

export type MyOrderSortBy = 'CreatedAt' | 'GrandTotal' | 'Status';

export interface GetMyOrdersQuery {
  status?: OrderStatus;
  shopId?: string;
  sortBy?: MyOrderSortBy;
  sortDirection?: 'Asc' | 'Desc';
  pageNumber?: number;
  pageSize?: number;
}

export interface PlaceOrderRequest {
  productId: string;
  quantity: number;
  shippingAddressId: string;
  specialInstructions?: string;
  attachmentPaths?: string[];
}

export interface CheckoutCartRequest {
  shippingAddressId: string;
  specialInstructions?: string;
}

export interface AcceptOrderQuoteRequest {
  method: PaymentMethod;
}
