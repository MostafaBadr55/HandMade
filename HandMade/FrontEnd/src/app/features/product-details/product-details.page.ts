import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { StorefrontService } from '../../core/storefront/storefront.service';
import { CartService } from '../../core/cart/cart.service';
import { OrderService } from '../../core/order/order.service';
import { AddressService } from '../../core/address/address.service';
import { Address } from '../../core/address/address.models';
import { FilesService } from '../../core/files/files.service';
import { ReviewService } from '../../core/review/review.service';
import { AuthTokenService } from '../../core/api/auth-token.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { apiErrorMessage } from '../../core/api/api-error';
import { resolveImageUrl } from '../../core/api/image-url';
import { ProductDetails } from '../../core/models/catalog.models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './product-details.page.html',
  styleUrls: ['./product-details.page.css']
})
export class ProductDetailsPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly storefrontService = inject(StorefrontService);
  private readonly cartService = inject(CartService);
  private readonly orderService = inject(OrderService);
  private readonly addressService = inject(AddressService);
  private readonly filesService = inject(FilesService);
  private readonly reviewService = inject(ReviewService);
  private readonly tokenService = inject(AuthTokenService);
  readonly i18n = inject(I18nService);
  lang = computed(() => this.i18n.lang());
  resolveImage = resolveImageUrl;

  productId = '';
  loading = signal(true);
  product = signal<ProductDetails | null>(null);
  activeImageIndex = signal(0);
  isClient = computed(() => this.tokenService.hasRole('Client'));

  // add to cart
  quantity = signal(1);
  addingToCart = signal(false);

  // request-a-quote form
  showQuoteForm = signal(false);
  addresses = signal<Address[]>([]);
  selectedAddressId = signal<string>('');
  specialInstructions = signal('');
  attachmentPaths = signal<string[]>([]);
  uploadingAttachment = signal(false);
  submittingQuote = signal(false);

  // review eligibility
  canReview = signal(false);
  showReviewForm = signal(false);
  reviewRating = signal(5);
  reviewTitle = signal('');
  reviewContent = signal('');
  submittingReview = signal(false);

  ngOnInit() {
    this.productId = this.route.snapshot.paramMap.get('productId') ?? '';
    if (!this.productId) return;

    this.storefrontService.getProductDetails(this.productId).subscribe({
      next: (p) => {
        this.product.set(p);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });

    if (this.isClient()) {
      this.reviewService.checkEligibility('Product', this.productId).subscribe({
        next: (res) => this.canReview.set(res.canReview),
        error: () => {}
      });
    }
  }

  addToCart() {
    if (!this.tokenService.hasRole('Client')) {
      this.router.navigate(['/login'], { queryParams: { next: `/product/${this.productId}` } });
      return;
    }
    this.addingToCart.set(true);
    this.cartService.addToCart(this.productId, this.quantity()).subscribe({
      next: () => {
        this.addingToCart.set(false);
        alert(this.lang() === 'ar' ? 'تم إضافة المنتج إلى السلة' : 'Added to cart');
      },
      error: (err) => {
        this.addingToCart.set(false);
        alert(apiErrorMessage(err));
      }
    });
  }

  openQuoteForm() {
    if (!this.tokenService.hasRole('Client')) {
      this.router.navigate(['/login'], { queryParams: { next: `/product/${this.productId}` } });
      return;
    }
    this.showQuoteForm.set(true);
    this.addressService.getMyAddresses().subscribe({
      next: (addrs) => {
        this.addresses.set(addrs);
        const def = addrs.find((a) => a.isDefault) ?? addrs[0];
        if (def) this.selectedAddressId.set(def.id);
      },
      error: () => {}
    });
  }

  onAttachmentSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.uploadingAttachment.set(true);
    this.filesService.upload(file, 'OrderAttachment').subscribe({
      next: (res) => {
        this.attachmentPaths.update((arr) => [...arr, res.relativePath]);
        this.uploadingAttachment.set(false);
        input.value = '';
      },
      error: (err) => {
        this.uploadingAttachment.set(false);
        alert(apiErrorMessage(err));
      }
    });
  }

  removeAttachment(index: number) {
    this.attachmentPaths.update((arr) => arr.filter((_, i) => i !== index));
  }

  submitQuoteRequest() {
    if (!this.selectedAddressId()) {
      alert(this.lang() === 'ar' ? 'الرجاء اختيار عنوان الشحن' : 'Please select a shipping address');
      return;
    }
    this.submittingQuote.set(true);
    this.orderService
      .placeOrderRequest({
        productId: this.productId,
        quantity: this.quantity(),
        shippingAddressId: this.selectedAddressId(),
        specialInstructions: this.specialInstructions() || undefined,
        attachmentPaths: this.attachmentPaths()
      })
      .subscribe({
        next: (res) => {
          this.submittingQuote.set(false);
          this.router.navigate(['/orders', res.orderId]);
        },
        error: (err) => {
          this.submittingQuote.set(false);
          alert(apiErrorMessage(err));
        }
      });
  }

  submitReview() {
    this.submittingReview.set(true);
    this.reviewService
      .createReview({
        targetType: 'Product',
        targetId: this.productId,
        rating: this.reviewRating(),
        title: this.reviewTitle(),
        content: this.reviewContent()
      })
      .subscribe({
        next: () => {
          this.submittingReview.set(false);
          this.showReviewForm.set(false);
          this.canReview.set(false);
          // Refresh the product to show the new review.
          this.storefrontService.getProductDetails(this.productId).subscribe((p) => this.product.set(p));
        },
        error: (err) => {
          this.submittingReview.set(false);
          alert(apiErrorMessage(err));
        }
      });
  }
}
