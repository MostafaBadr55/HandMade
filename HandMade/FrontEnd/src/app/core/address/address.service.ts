import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api/api.tokens';
import { Address, CreateAddressRequest, UpdateAddressRequest } from './address.models';

@Injectable({ providedIn: 'root' })
export class AddressService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getMyAddresses(): Observable<Address[]> {
    return this.http.get<Address[]>(`${this.apiBaseUrl}/api/Addresses`);
  }

  createAddress(request: CreateAddressRequest): Observable<void> {
    return this.http.post<void>(`${this.apiBaseUrl}/api/Addresses`, request);
  }

  updateAddress(addressId: string, request: UpdateAddressRequest): Observable<void> {
    return this.http.put<void>(`${this.apiBaseUrl}/api/Addresses/${addressId}`, request);
  }

  setDefault(addressId: string): Observable<void> {
    return this.http.patch<void>(`${this.apiBaseUrl}/api/Addresses/${addressId}/default`, null);
  }

  deleteAddress(addressId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/api/Addresses/${addressId}`);
  }
}
