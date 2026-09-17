import { Component, inject, signal, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIf, NgFor, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ShopService } from '../../core/shop/shop.service';
import { MyShop } from '../../core/shop/shop.models';
import { ProductManagementService } from '../../core/product/product-management.service';
import { FilesService } from '../../core/files/files.service';
import { StorefrontService } from '../../core/storefront/storefront.service';
import { Category } from '../../core/models/catalog.models';
import { I18nService } from '../../core/i18n/i18n.service';
import { apiErrorMessage } from '../../core/api/api-error';
import { ProductStatus } from '../../core/api/api.models';
import { SellerProduct } from '../../core/models/catalog.models';

import { ProductCardComponent } from '../../shared/components/product-card/product-card.component';

type ProductTabType = 'active' | 'inactive' | 'pending';

@Component({
  standalone: true,
  templateUrl: './my-shop.page.html',
  styleUrls: ['./my-shop.page.css'],
  imports: [NgIf, NgFor, CommonModule, FormsModule, RouterLink, ProductCardComponent]
})
export class MyShopPage {
  private readonly shopService = inject(ShopService);
  private readonly productService = inject(ProductManagementService);
  private readonly storefrontService = inject(StorefrontService);
  private readonly filesService = inject(FilesService);
  readonly i18n = inject(I18nService);
  private readonly router = inject(Router);

  protected readonly loading = signal(true);
  protected readonly serverError = signal<string | null>(null);
  protected readonly shop = signal<MyShop | null>(null);
  protected readonly categories = signal<Category[]>([]);

  protected readonly productTab = signal<ProductTabType>('active');
  protected readonly products = signal<SellerProduct[]>([]);
  protected readonly loadingProducts = signal(false);

  protected readonly lang = computed(() => this.i18n.lang());
  protected readonly isActive = computed(() => this.shop()?.status === 'Active');
  protected readonly isRejected = computed(() => this.shop()?.status === 'Rejected');
  protected readonly isPending = computed(() => this.shop()?.status === 'Pending');
  protected readonly isSuspended = computed(() => this.shop()?.status === 'Suspended');

  protected readonly showEditModal = signal(false);
  protected readonly editName = signal('');
  protected readonly editDescription = signal('');
  protected readonly editImageFile = signal<File | null>(null);
  protected readonly savingEdit = signal(false);

  protected readonly showAddProductModal = signal(false);
  protected readonly isAddingProduct = signal(false);
  protected readonly newTitle = signal('');
  protected readonly newDescription = signal('');
  protected readonly newPrice = signal(0);
  protected readonly newCategoryId = signal('');
  protected readonly newSubCategoryId = signal('');
  protected readonly newImageFile = signal<File | null>(null);
  protected readonly uploadInProgress = signal(false);

  protected readonly currentSubCategories = computed(() => {
    const catId = this.newCategoryId();
    return this.categories().find((c) => c.id === catId)?.subCategories ?? [];
  });

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.serverError.set(null);
    this.shopService.getMyShop().subscribe({
      next: (s) => {
        this.shop.set(s);
        this.loading.set(false);
        this.editName.set(s.name);
        this.editDescription.set(s.description);
        if (s.id) this.loadProducts();
      },
      error: (err) => {
        if (err.status === 404) {
          this.shop.set(null);
          this.serverError.set(null);
        } else {
          this.serverError.set(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل جلب المتجر.' : 'Failed to fetch shop.'));
        }
        this.loading.set(false);
      }
    });

    this.storefrontService.getCategories().subscribe({ next: (cats) => this.categories.set(cats) });
  }

  protected setProductTab(tab: ProductTabType) {
    this.productTab.set(tab);
    this.loadProducts();
  }

  private loadProducts(): void {
    const shopId = this.shop()?.id;
    if (!shopId) return;
    this.loadingProducts.set(true);
    const tab = this.productTab();
    const status: ProductStatus = tab === 'active' ? 'Active' : 'InActive';
    const criteria = tab === 'pending'
      ? { approvalStatus: 'Pending' as const, pageSize: 100 }
      : { status, approvalStatus: 'Approved' as const, pageSize: 100 };

    this.productService.getShopProducts(shopId, criteria).subscribe({
      next: (res) => {
        this.products.set(res.items ?? []);
        this.loadingProducts.set(false);
      },
      error: () => this.loadingProducts.set(false)
    });
  }

  onEditProduct(product: SellerProduct) {
    this.router.navigate(['/my-shop/add-product'], { queryParams: { productId: product.id } });
  }

  onDeleteProduct(product: SellerProduct) {
    this.productService.deleteProduct(product.id).subscribe({
      next: () => {
        this.loadProducts();
        alert(this.lang() === 'ar' ? 'تم حذف المنتج.' : 'Product deleted.');
      },
      error: (err) => alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل حذف المنتج.' : 'Failed to delete product.'))
    });
  }

  toggleProductStatus(product: SellerProduct) {
    const shopId = this.shop()?.id;
    if (!shopId) return;
    const newStatus = product.status === 'Active' ? 'InActive' : 'Active';

    this.productService.updateProductStatus(product.id, shopId, newStatus).subscribe({
      next: () => this.loadProducts(),
      error: (err) => alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل تغيير حالة المنتج.' : 'Failed to change product status.'))
    });
  }

  openAddProduct() {
    this.newTitle.set('');
    this.newDescription.set('');
    this.newPrice.set(0);
    this.newCategoryId.set('');
    this.newSubCategoryId.set('');
    this.newImageFile.set(null);
    this.showAddProductModal.set(true);
  }

  closeAddProductModal() {
    this.showAddProductModal.set(false);
  }

  onAddProductFileChange(ev: Event) {
    const input = ev.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) this.newImageFile.set(file);
  }

  submitAddProduct() {
    const shopId = this.shop()?.id;
    if (!shopId) return;

    if (!this.newTitle() || !this.newDescription() || !this.newPrice() || !this.newCategoryId() || !this.newSubCategoryId()) {
      alert(this.lang() === 'ar' ? 'يرجى ملء جميع الحقول المطلوبة.' : 'Please fill all required fields.');
      return;
    }

    this.isAddingProduct.set(true);
    const file = this.newImageFile();

    const create = (images: { relativePath: string; altText: string; isPrimary: boolean; sortOrder: number }[]) => {
      this.productService.createProduct({
        shopId,
        categoryId: this.newCategoryId(),
        subCategoryId: this.newSubCategoryId(),
        title: this.newTitle(),
        description: this.newDescription(),
        price: this.newPrice(),
        images
      }).subscribe({
        next: () => {
          this.isAddingProduct.set(false);
          this.showAddProductModal.set(false);
          this.loadProducts();
          alert(this.lang() === 'ar' ? 'تم إضافة المنتج بنجاح! بانتظار موافقة المشرف.' : 'Product added successfully! Awaiting admin approval.');
        },
        error: (err) => {
          this.isAddingProduct.set(false);
          alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل إضافة المنتج.' : 'Failed to add product.'));
        }
      });
    };

    if (file) {
      this.uploadInProgress.set(true);
      this.filesService.upload(file, 'Product').subscribe({
        next: (res) => {
          this.uploadInProgress.set(false);
          create([{ relativePath: res.relativePath, altText: this.newTitle(), isPrimary: true, sortOrder: 0 }]);
        },
        error: (err) => {
          this.uploadInProgress.set(false);
          this.isAddingProduct.set(false);
          alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل رفع الصورة.' : 'Failed to upload image.'));
        }
      });
    } else {
      create([]);
    }
  }

  createShop() {
    this.router.navigate(['/create-shop']);
  }

  openEditModal() {
    const s = this.shop();
    if (s) {
      this.editName.set(s.name);
      this.editDescription.set(s.description);
      this.editImageFile.set(null);
      this.showEditModal.set(true);
    }
  }

  closeEditModal() {
    this.showEditModal.set(false);
  }

  onEditFileChange(ev: Event) {
    const input = ev.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) this.editImageFile.set(file);
  }

  submitEdit() {
    const s = this.shop();
    if (!s) return;

    this.savingEdit.set(true);
    const file = this.editImageFile();

    const update = (imageRelativePath: string) => {
      this.shopService.updateShop({
        shopId: s.id,
        shopName: this.editName(),
        shopDescription: this.editDescription(),
        imageRelativePath
      }).subscribe({
        next: () => {
          this.savingEdit.set(false);
          this.showEditModal.set(false);
          this.load();
          alert(this.lang() === 'ar' ? 'تم حفظ التغييرات بنجاح.' : 'Changes saved successfully.');
        },
        error: (err) => {
          this.savingEdit.set(false);
          alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل تحديث المتجر.' : 'Failed to update shop.'));
        }
      });
    };

    if (file) {
      this.filesService.upload(file, 'Shop').subscribe({
        next: (res) => update(res.relativePath),
        error: (err) => {
          this.savingEdit.set(false);
          alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل رفع الصورة.' : 'Failed to upload image.'));
        }
      });
    } else {
      // Keep the current image if the artist did not pick a new one.
      update(s.imageUrl);
    }
  }
}
