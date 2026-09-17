import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CartService, Cart } from '../../core/cart/cart.service';
import { AddressService } from '../../core/address/address.service';
import { Address } from '../../core/address/address.models';
import { OrderService } from '../../core/order/order.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { apiErrorMessage } from '../../core/api/api-error';
import { resolveImageUrl } from '../../core/api/image-url';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './checkout.page.html',
  styleUrls: ['./checkout.page.css']
})
export class CheckoutPage implements OnInit {
  private readonly cartService = inject(CartService);
  private readonly addressService = inject(AddressService);
  private readonly orderService = inject(OrderService);
  private readonly router = inject(Router);
  readonly i18n = inject(I18nService);
  lang = computed(() => this.i18n.lang());
  resolveImage = resolveImageUrl;

  loading = signal(true);
  cart = signal<Cart | null>(null);
  addresses = signal<Address[]>([]);
  selectedAddressId = signal('');
  specialInstructions = signal('');
  submitting = signal(false);
  error = signal<string | null>(null);

  ngOnInit() {
    this.cartService.getCart().subscribe({
      next: (cart) => {
        this.cart.set(cart);
        this.loading.set(false);
        if (!cart.items.length) {
          this.router.navigate(['/cart']);
        }
      },
      error: () => this.loading.set(false)
    });

    this.addressService.getMyAddresses().subscribe({
      next: (addrs) => {
        this.addresses.set(addrs);
        const def = addrs.find((a) => a.isDefault) ?? addrs[0];
        if (def) this.selectedAddressId.set(def.id);
      },
      error: () => {}
    });
  }

  placeOrder() {
    if (!this.selectedAddressId()) {
      this.error.set(this.lang() === 'ar' ? 'الرجاء اختيار عنوان الشحن' : 'Please select a shipping address');
      return;
    }
    this.error.set(null);
    this.submitting.set(true);

    this.orderService
      .checkout({
        shippingAddressId: this.selectedAddressId(),
        specialInstructions: this.specialInstructions() || undefined
      })
      .subscribe({
        next: () => {
          this.submitting.set(false);
          this.router.navigate(['/orders']);
        },
        error: (err) => {
          this.submitting.set(false);
          this.error.set(apiErrorMessage(err));
        }
      });
  }
}
