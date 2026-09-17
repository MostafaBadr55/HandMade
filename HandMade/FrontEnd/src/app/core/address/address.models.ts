export interface Address {
  id: string;
  label: string;
  detailedAddress: string;
  isDefault: boolean;
}

export interface CreateAddressRequest {
  label: string;
  detailedAddress: string;
  isDefault?: boolean;
}

export interface UpdateAddressRequest {
  label: string;
  detailedAddress: string;
}
