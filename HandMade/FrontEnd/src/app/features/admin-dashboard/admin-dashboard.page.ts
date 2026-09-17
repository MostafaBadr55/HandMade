import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { AdminService } from '../../core/admin/admin.service';
import { AdminShop, AdminProduct, ShopSortBy } from '../../core/admin/admin.models';
import { FilesService } from '../../core/files/files.service';
import { Category, SubCategory } from '../../core/models/catalog.models';
import { ProductApprovalStatus, ProductStatus, ShopStatus } from '../../core/api/api.models';
import { I18nService } from '../../core/i18n/i18n.service';
import { TPipe } from '../../core/i18n/t.pipe';
import { apiErrorMessage } from '../../core/api/api-error';
import { resolveImageUrl } from '../../core/api/image-url';

type TabType = 'shops' | 'products' | 'categories' | 'subcategories';

interface SubCategoryRow extends SubCategory {
  categoryName: string;
}

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TPipe],
  templateUrl: './admin-dashboard.page.html',
  styleUrl: './admin-dashboard.page.css'
})
export class AdminDashboardPage {
  private readonly adminService = inject(AdminService);
  private readonly filesService = inject(FilesService);
  readonly i18n = inject(I18nService);
  lang = computed(() => this.i18n.lang());
  resolveImage = resolveImageUrl;
  private searchTimeout: ReturnType<typeof setTimeout> | null = null;

  activeTab = signal<TabType>('shops');
  loading = signal(false);
  error = signal<string | null>(null);
  success = signal<string | null>(null);

  // Shops
  shops = signal<AdminShop[]>([]);
  pendingShops = signal<AdminShop[]>([]);
  shopFilters = signal<{ status: ShopStatus | ''; name: string }>({ status: '', name: '' });

  // Products
  products = signal<AdminProduct[]>([]);
  productFilters = signal<{ approvalStatus: ProductApprovalStatus | ''; status: ProductStatus | '' }>({
    approvalStatus: '',
    status: ''
  });

  // Categories
  categories = signal<Category[]>([]);
  categoryForm = signal({ id: '', name: '', description: '', imageUrl: '' });
  editingCategory = signal(false);
  uploadInProgress = signal(false);

  // SubCategories (derived from categories())
  subCategoryRows = computed<SubCategoryRow[]>(() =>
    this.categories().flatMap((c) => (c.subCategories ?? []).map((sc) => ({ ...sc, categoryName: c.name })))
  );
  subCategoryForm = signal({ id: '', categoryId: '', name: '' });
  editingSubCategory = signal(false);

  // Reject Modal
  showRejectModal = signal(false);
  rejectType = signal<'shop' | 'product'>('shop');
  rejectId = signal('');
  rejectMessage = signal('');

  constructor() {
    this.loadShops();
  }

  setActiveTab(tab: TabType) {
    this.activeTab.set(tab);
    this.error.set(null);
    this.success.set(null);

    switch (tab) {
      case 'shops': this.loadShops(); break;
      case 'products': this.loadProducts(); break;
      case 'categories': this.loadCategories(); break;
      case 'subcategories': this.loadCategories(); break;
    }
  }

  // ============ SHOPS ============
  loadShops() {
    this.loadAllShops();
    this.loadPendingShops();
  }

  loadAllShops() {
    const filters = this.shopFilters();
    this.adminService.getShops({
      status: filters.status || undefined,
      name: filters.name || undefined,
      sortBy: 'CreatedAt' as ShopSortBy,
      pageNumber: 1,
      pageSize: 50
    }).subscribe({
      next: (res) => this.shops.set(res.items ?? []),
      error: () => this.error.set(this.lang() === 'ar' ? 'فشل تحميل المتاجر' : 'Failed to load shops')
    });
  }

  loadPendingShops() {
    this.adminService.getShops({ status: 'Pending', pageNumber: 1, pageSize: 50 }).subscribe({
      next: (res) => this.pendingShops.set(res.items ?? []),
      error: () => {}
    });
  }

  approveShop(shopId: string) {
    this.adminService.approveShop(shopId).subscribe({
      next: () => {
        this.success.set(this.lang() === 'ar' ? 'تمت الموافقة على المتجر!' : 'Shop approved successfully!');
        this.pendingShops.update((shops) => shops.filter((s) => s.id !== shopId));
        this.loadAllShops();
      },
      error: (err) => this.error.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل الموافقة على المتجر' : 'Failed to approve shop'))
    });
  }

  openRejectShopModal(shopId: string) {
    this.rejectType.set('shop');
    this.rejectId.set(shopId);
    this.rejectMessage.set('');
    this.showRejectModal.set(true);
  }

  // ============ PRODUCTS ============
  loadProducts() {
    this.loading.set(true);
    const filters = this.productFilters();
    this.adminService.getProducts({
      approvalStatus: filters.approvalStatus || undefined,
      status: filters.status || undefined,
      pageNumber: 1,
      pageSize: 50
    }).subscribe({
      next: (res) => {
        this.products.set(res.items ?? []);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل تحميل المنتجات' : 'Failed to load products'));
        this.loading.set(false);
      }
    });
  }

  approveProduct(productId: string) {
    this.adminService.approveProduct(productId).subscribe({
      next: () => {
        this.success.set(this.lang() === 'ar' ? 'تمت الموافقة على المنتج!' : 'Product approved successfully!');
        this.products.update((products) =>
          products.map((p) => (p.id === productId ? { ...p, status: 'Active' as ProductStatus } : p))
        );
        this.loadProducts();
      },
      error: (err) => this.error.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل الموافقة على المنتج' : 'Failed to approve product'))
    });
  }

  openRejectProductModal(productId: string) {
    this.rejectType.set('product');
    this.rejectId.set(productId);
    this.rejectMessage.set('');
    this.showRejectModal.set(true);
  }

  // ============ REJECT MODAL ============
  confirmReject() {
    const type = this.rejectType();
    const id = this.rejectId();
    const message = this.rejectMessage();

    const request$ = type === 'shop'
      ? this.adminService.rejectShop(id, message)
      : this.adminService.rejectProduct(id, message);

    request$.subscribe({
      next: () => {
        this.success.set(type === 'shop'
          ? (this.lang() === 'ar' ? 'تم رفض المتجر!' : 'Shop rejected successfully!')
          : (this.lang() === 'ar' ? 'تم رفض المنتج!' : 'Product rejected successfully!'));
        this.showRejectModal.set(false);
        if (type === 'shop') this.loadShops(); else this.loadProducts();
      },
      error: (err) => this.error.set(apiErrorMessage(err))
    });
  }

  // ============ CATEGORIES ============
  loadCategories() {
    this.loading.set(true);
    this.adminService.getCategories().subscribe({
      next: (res) => {
        this.categories.set(res.items ?? []);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل تحميل التصنيفات' : 'Failed to load categories'));
        this.loading.set(false);
      }
    });
  }

  saveCategory() {
    const form = this.categoryForm();
    if (!form.name) {
      this.error.set(this.lang() === 'ar' ? 'اسم التصنيف مطلوب' : 'Category name is required');
      return;
    }

    const payload = { name: form.name, description: form.description, imageUrl: form.imageUrl };
    const request$: Observable<unknown> = this.editingCategory()
      ? this.adminService.updateCategory(form.id, payload)
      : this.adminService.createCategory(payload);

    request$.subscribe({
      next: () => {
        this.success.set(this.editingCategory()
          ? (this.lang() === 'ar' ? 'تم تحديث التصنيف!' : 'Category updated!')
          : (this.lang() === 'ar' ? 'تم إنشاء التصنيف!' : 'Category created!'));
        this.resetCategoryForm();
        this.loadCategories();
      },
      error: (err) => this.error.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل حفظ التصنيف' : 'Failed to save category'))
    });
  }

  editCategory(cat: Category) {
    this.categoryForm.set({ id: cat.id, name: cat.name, description: cat.description ?? '', imageUrl: cat.imageUrl ?? '' });
    this.editingCategory.set(true);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  deleteCategory(id: string) {
    if (!confirm(this.lang() === 'ar' ? 'هل أنت متأكد من حذف هذا التصنيف؟' : 'Are you sure you want to delete this category?')) return;

    this.adminService.deleteCategory(id).subscribe({
      next: () => {
        this.success.set(this.lang() === 'ar' ? 'تم حذف التصنيف!' : 'Category deleted!');
        this.loadCategories();
      },
      error: (err) => this.error.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل حذف التصنيف' : 'Failed to delete category'))
    });
  }

  resetCategoryForm() {
    this.categoryForm.set({ id: '', name: '', description: '', imageUrl: '' });
    this.editingCategory.set(false);
  }

  // ============ SUBCATEGORIES ============
  saveSubCategory() {
    const form = this.subCategoryForm();
    if (!form.name || !form.categoryId) {
      this.error.set(this.lang() === 'ar' ? 'الاسم والتصنيف مطلوبان' : 'Name and Category are required');
      return;
    }

    const payload = { categoryId: form.categoryId, name: form.name };
    const request$: Observable<unknown> = this.editingSubCategory()
      ? this.adminService.updateSubCategory(form.id, payload)
      : this.adminService.createSubCategory(payload);

    request$.subscribe({
      next: () => {
        this.success.set(this.editingSubCategory()
          ? (this.lang() === 'ar' ? 'تم تحديث التصنيف الفرعي!' : 'SubCategory updated!')
          : (this.lang() === 'ar' ? 'تم إنشاء التصنيف الفرعي!' : 'SubCategory created!'));
        this.resetSubCategoryForm();
        this.loadCategories();
      },
      error: (err) => this.error.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل حفظ التصنيف الفرعي' : 'Failed to save subcategory'))
    });
  }

  editSubCategory(sub: SubCategoryRow) {
    this.subCategoryForm.set({ id: sub.id, categoryId: sub.categoryId, name: sub.name });
    this.editingSubCategory.set(true);
  }

  deleteSubCategory(id: string) {
    if (!confirm(this.lang() === 'ar' ? 'هل أنت متأكد من حذف هذا التصنيف الفرعي؟' : 'Are you sure you want to delete this subcategory?')) return;

    this.adminService.deleteSubCategory(id).subscribe({
      next: () => {
        this.success.set(this.lang() === 'ar' ? 'تم حذف التصنيف الفرعي!' : 'SubCategory deleted!');
        this.loadCategories();
      },
      error: (err) => this.error.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل حذف التصنيف الفرعي' : 'Failed to delete subcategory'))
    });
  }

  resetSubCategoryForm() {
    this.subCategoryForm.set({ id: '', categoryId: '', name: '' });
    this.editingSubCategory.set(false);
  }

  // ============ FORM HELPER METHODS ============
  updateShopName(value: string) {
    this.shopFilters.update((f) => ({ ...f, name: value }));
    if (this.searchTimeout) clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => this.loadAllShops(), 400);
  }

  updateShopStatus(value: ShopStatus | '') {
    this.shopFilters.update((f) => ({ ...f, status: value }));
    this.loadAllShops();
  }

  updateProductApprovalStatus(value: ProductApprovalStatus | '') {
    this.productFilters.update((f) => ({ ...f, approvalStatus: value }));
    this.loadProducts();
  }

  updateProductStatus(value: ProductStatus | '') {
    this.productFilters.update((f) => ({ ...f, status: value }));
    this.loadProducts();
  }

  updateCategoryName(value: string) {
    this.categoryForm.update((f) => ({ ...f, name: value }));
  }

  updateCategoryDescription(value: string) {
    this.categoryForm.update((f) => ({ ...f, description: value }));
  }

  onCategoryFileChange(ev: Event) {
    const input = ev.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.uploadInProgress.set(true);
    this.filesService.upload(file, 'Category').subscribe({
      next: (res) => {
        this.categoryForm.update((f) => ({ ...f, imageUrl: res.relativePath }));
        this.uploadInProgress.set(false);
      },
      error: (err) => {
        this.uploadInProgress.set(false);
        this.error.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل رفع الصورة' : 'Failed to upload image'));
      }
    });
  }

  updateSubCategoryCategoryId(value: string) {
    this.subCategoryForm.update((f) => ({ ...f, categoryId: value }));
  }

  updateSubCategoryName(value: string) {
    this.subCategoryForm.update((f) => ({ ...f, name: value }));
  }

  updateRejectMessage(value: string) {
    this.rejectMessage.set(value);
  }
}
