import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { OrderService } from '../../../core/order/order.service';
import { OrderDetails } from '../../../core/order/order.models';
import { ReviewService } from '../../../core/review/review.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { apiErrorMessage } from '../../../core/api/api-error';
import { resolveImageUrl } from '../../../core/api/image-url';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './order-details.page.html',
  styleUrls: ['./order-details.page.css']
})
export class OrderDetailsPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly orderService = inject(OrderService);
  private readonly reviewService = inject(ReviewService);
  readonly i18n = inject(I18nService);
  lang = computed(() => this.i18n.lang());
  resolveImage = resolveImageUrl;

  orderId = '';
  loading = signal(true);
  order = signal<OrderDetails | null>(null);
  acting = signal(false);

  canReview = signal(false);
  showReviewForm = signal(false);
  reviewRating = signal(5);
  reviewTitle = signal('');
  reviewContent = signal('');
  submittingReview = signal(false);

  ngOnInit() {
    this.orderId = this.route.snapshot.paramMap.get('orderId') ?? '';
    if (!this.orderId) return;
    this.load();
  }

  load() {
    this.loading.set(true);
    this.orderService.getOrder(this.orderId).subscribe({
      next: (o) => {
        this.order.set(o);
        this.loading.set(false);
        if (o.status === 'Delivered') {
          this.reviewService.checkEligibility('Product', o.productId).subscribe({
            next: (res) => this.canReview.set(res.canReview),
            error: () => {}
          });
        }
      },
      error: (err) => {
        this.loading.set(false);
        alert(apiErrorMessage(err));
        this.router.navigate(['/orders']);
      }
    });
  }

  acceptQuote() {
    if (!confirm(this.lang() === 'ar' ? 'سيتم خصم المبلغ من حسابك الآن. هل تريد المتابعة؟' : 'Your card will be charged now. Continue?')) return;
    this.acting.set(true);
    this.orderService.acceptQuote(this.orderId).subscribe({
      next: () => { this.acting.set(false); this.load(); },
      error: (err) => { this.acting.set(false); alert(apiErrorMessage(err)); }
    });
  }

  rejectQuote() {
    const reason = prompt(this.lang() === 'ar' ? 'سبب الرفض (اختياري)' : 'Reason (optional)') ?? undefined;
    this.acting.set(true);
    this.orderService.rejectQuote(this.orderId, reason).subscribe({
      next: () => { this.acting.set(false); this.load(); },
      error: (err) => { this.acting.set(false); alert(apiErrorMessage(err)); }
    });
  }

  cancelOrder() {
    const reason = prompt(this.lang() === 'ar' ? 'سبب الإلغاء (اختياري)' : 'Reason (optional)') ?? undefined;
    this.acting.set(true);
    this.orderService.cancelOrder(this.orderId, reason).subscribe({
      next: () => { this.acting.set(false); this.load(); },
      error: (err) => { this.acting.set(false); alert(apiErrorMessage(err)); }
    });
  }

  confirmDelivery() {
    if (!confirm(this.lang() === 'ar' ? 'هل استلمت الطلب بحالة جيدة؟' : 'Did you receive the order in good condition?')) return;
    this.acting.set(true);
    this.orderService.confirmDelivery(this.orderId).subscribe({
      next: () => { this.acting.set(false); this.load(); },
      error: (err) => { this.acting.set(false); alert(apiErrorMessage(err)); }
    });
  }

  submitReview() {
    const o = this.order();
    if (!o) return;
    this.submittingReview.set(true);
    this.reviewService.createReview({
      targetType: 'Product',
      targetId: o.productId,
      rating: this.reviewRating(),
      title: this.reviewTitle(),
      content: this.reviewContent()
    }).subscribe({
      next: () => {
        this.submittingReview.set(false);
        this.showReviewForm.set(false);
        this.canReview.set(false);
      },
      error: (err) => {
        this.submittingReview.set(false);
        alert(apiErrorMessage(err));
      }
    });
  }
}
