import { AssignedRole } from '../api/api.models';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  // The typo is on the wire (RegistrationRequestVM.AddressLable) — kept as-is
  // deliberately so the JSON key matches what the API binds to.
  addressLable?: string;
  address?: string;
  phone?: string;
}

export interface AuthResponse {
  token: string;
  userId: string;
}

export interface SelectRoleRequest {
  username: string;
  selectedRole: AssignedRole;
}

export interface SelectRoleResponse {
  role: AssignedRole;
  username: string;
  message: string;
}
