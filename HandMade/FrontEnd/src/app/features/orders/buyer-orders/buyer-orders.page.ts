import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { OrderService } from '../../../core/order/order.service';
import { OrderListItem } from '../../../core/order/order.models';
import { OrderStatus } from '../../../core/api/api.models';
import { I18nService } from '../../../core/i18n/i18n.service';
import { apiErrorMessage } from '../../../core/api/api-error';
import { resolveImageUrl } from '../../../core/api/image-url';

const PENDING_STATUSES: OrderStatus[] = ['SellerPending', 'BuyerPending'];
const ACTIVE_STATUSES: OrderStatus[] = ['InProgress', 'CompletedBySeller'];
const HISTORY_STATUSES: OrderStatus[] = ['Delivered', 'Cancelled', 'Refunded'];

@Component({
    standalone: true,
    selector: 'app-buyer-orders',
    templateUrl: './buyer-orders.page.html',
    styleUrls: ['./buyer-orders.page.css'],
    imports: [CommonModule, FormsModule, RouterLink, CurrencyPipe, DatePipe]
})
export class BuyerOrdersPage implements OnInit {
    private readonly orderService = inject(OrderService);
    readonly i18n = inject(I18nService);
    readonly lang = computed(() => this.i18n.lang());
    readonly resolveImage = resolveImageUrl;

    private allOrders = signal<OrderListItem[]>([]);
    loading = signal(false);
    error = signal<string | null>(null);

    activeTab = signal<'pending' | 'active' | 'history'>('pending');

    orders = computed(() => {
        const tab = this.activeTab();
        const statuses = tab === 'pending' ? PENDING_STATUSES : tab === 'active' ? ACTIVE_STATUSES : HISTORY_STATUSES;
        return this.allOrders().filter((o) => statuses.includes(o.status));
    });

    // Cancel / reject modal
    showCancelModal = signal(false);
    selectedOrder = signal<OrderListItem | null>(null);
    cancelReason = signal('');
    cancelling = signal(false);
    cancelMode = signal<'cancel' | 'reject'>('cancel');

    acceptingId = signal<string | null>(null);
    confirmingId = signal<string | null>(null);

    ngOnInit() {
        this.loadOrders();
    }

    setTab(tab: 'pending' | 'active' | 'history') {
        this.activeTab.set(tab);
    }

    loadOrders() {
        this.loading.set(true);
        this.error.set(null);

        this.orderService.getMyOrders({ sortBy: 'CreatedAt', sortDirection: 'Desc', pageNumber: 1, pageSize: 100 }).subscribe({
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

    acceptQuote(order: OrderListItem) {
        if (!confirm(this.lang() === 'ar' ? 'سيتم خصم المبلغ من حسابك الآن. هل تريد المتابعة؟' : 'Your card will be charged now. Continue?')) return;
        this.acceptingId.set(order.orderId);
        this.orderService.acceptQuote(order.orderId).subscribe({
            next: () => {
                this.acceptingId.set(null);
                this.loadOrders();
            },
            error: (err) => {
                this.acceptingId.set(null);
                alert(apiErrorMessage(err));
            }
        });
    }

    openRejectModal(order: OrderListItem) {
        this.cancelMode.set('reject');
        this.selectedOrder.set(order);
        this.cancelReason.set('');
        this.showCancelModal.set(true);
    }

    openCancelModal(order: OrderListItem) {
        this.cancelMode.set('cancel');
        this.selectedOrder.set(order);
        this.cancelReason.set('');
        this.showCancelModal.set(true);
    }

    closeCancelModal() {
        this.showCancelModal.set(false);
        this.selectedOrder.set(null);
    }

    submitCancel() {
        const order = this.selectedOrder();
        if (!order) return;

        this.cancelling.set(true);
        const request$ = this.cancelMode() === 'reject'
            ? this.orderService.rejectQuote(order.orderId, this.cancelReason())
            : this.orderService.cancelOrder(order.orderId, this.cancelReason());

        request$.subscribe({
            next: () => {
                this.cancelling.set(false);
                this.closeCancelModal();
                this.loadOrders();
            },
            error: (err) => {
                this.cancelling.set(false);
                alert(apiErrorMessage(err));
            }
        });
    }

    confirmDelivery(order: OrderListItem) {
        if (!confirm(this.lang() === 'ar' ? 'هل استلمت الطلب بحالة جيدة؟' : 'Did you receive the order in good condition?')) return;
        this.confirmingId.set(order.orderId);
        this.orderService.confirmDelivery(order.orderId).subscribe({
            next: () => {
                this.confirmingId.set(null);
                this.loadOrders();
            },
            error: (err) => {
                this.confirmingId.set(null);
                alert(apiErrorMessage(err));
            }
        });
    }

    getStatusLabel(status: OrderStatus): string {
        const labels: Record<string, { ar: string, en: string }> = {
            SellerPending: { ar: 'بانتظار عرض السعر', en: 'Awaiting Quote' },
            BuyerPending: { ar: 'بانتظار موافقتك', en: 'Action Required' },
            InProgress: { ar: 'قيد التنفيذ', en: 'In Progress' },
            CompletedBySeller: { ar: 'جاهز للتسليم', en: 'Ready for Delivery' },
            Delivered: { ar: 'تم التسليم', en: 'Delivered' },
            Cancelled: { ar: 'ملغي', en: 'Cancelled' },
            Refunded: { ar: 'مسترجع', en: 'Refunded' }
        };
        return this.lang() === 'ar' ? (labels[status]?.ar || status) : (labels[status]?.en || status);
    }
}
