import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderManagementService, ShopOrderListItem } from '../../core/order/order-management.service';
import { OrderStatus } from '../../core/api/api.models';
import { I18nService } from '../../core/i18n/i18n.service';
import { apiErrorMessage } from '../../core/api/api-error';
import { resolveImageUrl } from '../../core/api/image-url';

const PENDING_STATUSES: OrderStatus[] = ['SellerPending'];
const ACTIVE_STATUSES: OrderStatus[] = ['BuyerPending', 'InProgress', 'CompletedBySeller'];
const HISTORY_STATUSES: OrderStatus[] = ['Delivered', 'Cancelled', 'Refunded'];

@Component({
  standalone: true,
  selector: 'app-artist-orders',
  imports: [CommonModule, FormsModule],
  templateUrl: './artist-orders.page.html',
  styleUrls: ['./artist-orders.page.css']
})
export class ArtistOrdersPage implements OnInit {
  private readonly orderManagementService = inject(OrderManagementService);
  readonly i18n = inject(I18nService);
  lang = computed(() => this.i18n.lang());
  resolveImage = resolveImageUrl;

  private allOrders = signal<ShopOrderListItem[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  activeTab = signal<'pending' | 'active' | 'history'>('pending');

  orders = computed(() => {
    const tab = this.activeTab();
    const statuses = tab === 'pending' ? PENDING_STATUSES : tab === 'active' ? ACTIVE_STATUSES : HISTORY_STATUSES;
    return this.allOrders().filter((o) => statuses.includes(o.status));
  });

  // Quote modal
  showQuoteModal = signal(false);
  selectedOrder = signal<ShopOrderListItem | null>(null);
  quotePrice = signal(0);
  quoteDays = signal(3);
  submittingQuote = signal(false);

  processingId = signal<string | null>(null);

  ngOnInit() {
    this.load();
  }

  setTab(tab: 'pending' | 'active' | 'history') {
    this.activeTab.set(tab);
  }

  load() {
    this.loading.set(true);
    this.error.set(null);
    this.orderManagementService.getShopOrders({ sortBy: 'CreatedAt', sortDirection: 'Desc', pageNumber: 1, pageSize: 100 }).subscribe({
      next: (res) => {
        this.allOrders.set(res.items ?? []);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل تحميل الطلبات' : 'Failed to load orders'));
        this.loading.set(false);
      }
    });
  }

  openQuoteModal(order: ShopOrderListItem) {
    this.selectedOrder.set(order);
    this.quotePrice.set(0);
    this.quoteDays.set(3);
    this.showQuoteModal.set(true);
  }

  closeQuoteModal() {
    this.showQuoteModal.set(false);
    this.selectedOrder.set(null);
  }

  submitQuote() {
    const order = this.selectedOrder();
    if (!order || this.quotePrice() <= 0 || this.quoteDays() < 1) return;

    this.submittingQuote.set(true);
    this.orderManagementService.submitQuote(order.orderId, this.quotePrice(), this.quoteDays()).subscribe({
      next: () => {
        this.submittingQuote.set(false);
        this.closeQuoteModal();
        this.load();
      },
      error: (err) => {
        this.submittingQuote.set(false);
        alert(apiErrorMessage(err));
      }
    });
  }

  rejectRequest(order: ShopOrderListItem) {
    const reason = prompt(this.lang() === 'ar' ? 'سبب الرفض (اختياري)' : 'Reason (optional)') ?? undefined;
    this.processingId.set(order.orderId);
    this.orderManagementService.rejectRequest(order.orderId, reason).subscribe({
      next: () => {
        this.processingId.set(null);
        this.load();
      },
      error: (err) => {
        this.processingId.set(null);
        alert(apiErrorMessage(err));
      }
    });
  }

  markComplete(order: ShopOrderListItem) {
    if (!confirm(this.lang() === 'ar' ? 'هل انتهيت من العمل على هذا الطلب؟' : 'Have you finished working on this order?')) return;
    this.processingId.set(order.orderId);
    this.orderManagementService.markComplete(order.orderId).subscribe({
      next: () => {
        this.processingId.set(null);
        this.load();
      },
      error: (err) => {
        this.processingId.set(null);
        alert(apiErrorMessage(err));
      }
    });
  }

  getStatusLabel(status: OrderStatus): string {
    const labels: Record<string, { ar: string; en: string }> = {
      SellerPending: { ar: 'يحتاج عرض سعر', en: 'Needs a Quote' },
      BuyerPending: { ar: 'بانتظار العميل', en: 'Awaiting Client' },
      InProgress: { ar: 'قيد التنفيذ', en: 'In Progress' },
      CompletedBySeller: { ar: 'بانتظار تأكيد الاستلام', en: 'Awaiting Confirmation' },
      Delivered: { ar: 'تم التسليم', en: 'Delivered' },
      Cancelled: { ar: 'ملغي', en: 'Cancelled' },
      Refunded: { ar: 'مسترجع', en: 'Refunded' }
    };
    return this.lang() === 'ar' ? (labels[status]?.ar || status) : (labels[status]?.en || status);
  }
}
