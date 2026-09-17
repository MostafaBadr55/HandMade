// Shared wire types for the HandMade API. All enums serialize as STRINGS
// (JsonStringEnumConverter in Program.cs) and every id is a Guid (string).

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

/** Domain/business error from ErrorCodeExtensions.ToProblem — plain application/json. */
export interface ApiProblem {
  title: string;
  detail: string;
  status: number;
  instance: string | null;
  errorCode: number;
}

/** ASP.NET [ApiController] model-state failure — application/problem+json, a DIFFERENT shape. */
export interface ValidationProblem {
  type?: string;
  title: string;
  status: number;
  traceId?: string;
  errors: Record<string, string[]>;
}

export type SortDirection = 'Asc' | 'Desc';

export type AssignedRole = 'Admin' | 'Artist' | 'Client';

export type OrderStatus =
  | 'SellerPending'
  | 'BuyerPending'
  | 'InProgress'
  | 'CompletedBySeller'
  | 'Delivered'
  | 'Cancelled'
  | 'Refunded';

export type EscrowStatus = 'None' | 'Held' | 'Released' | 'Refunded';

export type PaymentStatus = 'Pending' | 'Processing' | 'Completed' | 'Failed' | 'Refunded' | 'Cancelled';

export type PaymentMethod = 'CreditCard' | 'DebitCard' | 'PayPal' | 'BankTransfer' | 'CashOnDelivery';

export type ShopStatus = 'Active' | 'Inactive' | 'Pending' | 'Suspended' | 'Rejected';

/** Note the capital A mid-word — this is NOT "Inactive". */
export type ProductStatus = 'Active' | 'InActive';

export type ProductApprovalStatus = 'Pending' | 'Approved' | 'Rejected';

export type ReviewTargetType = 'Product' | 'Shop' | 'Buyer';

export type UploadTarget = 'Shop' | 'Product' | 'OrderAttachment' | 'Category';
