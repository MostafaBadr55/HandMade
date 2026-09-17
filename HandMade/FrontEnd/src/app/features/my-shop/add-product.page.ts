import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ShopService } from '../../core/shop/shop.service';
import { ProductManagementService } from '../../core/product/product-management.service';
import { FilesService } from '../../core/files/files.service';
import { StorefrontService } from '../../core/storefront/storefront.service';
import { Category, SubCategory } from '../../core/models/catalog.models';
import { I18nService } from '../../core/i18n/i18n.service';
import { apiErrorMessage } from '../../core/api/api-error';
import { resolveImageUrl } from '../../core/api/image-url';

@Component({
  standalone: true,
  templateUrl: './add-product.page.html',
  styleUrl: './add-product.page.css',
  imports: [CommonModule, FormsModule]
})
export class AddProductPage implements OnInit {
  private readonly shopService = inject(ShopService);
  private readonly productService = inject(ProductManagementService);
  private readonly storefrontService = inject(StorefrontService);
  private readonly filesService = inject(FilesService);
  private readonly i18n = inject(I18nService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected readonly categories = signal<Category[]>([]);
  protected readonly isEdit = signal(false);
  protected readonly shopId = signal<string | null>(null);
  protected readonly loading = signal(false);
  protected readonly lang = computed(() => this.i18n.lang());
  protected readonly resolveImage = resolveImageUrl;

  protected readonly currentSubCategories = computed(() => {
    const catId = this.form.categoryId;
    return this.categories().find((c) => c.id === catId)?.subCategories ?? [];
  });

  protected form: {
    title: string;
    description: string;
    price: number;
    categoryId: string;
    subCategoryId: string;
    imageRelativePath: string;
    imageUrl: string;
  } = {
    title: '',
    description: '',
    price: 0,
    categoryId: '',
    subCategoryId: '',
    imageRelativePath: '',
    imageUrl: ''
  };

  protected readonly uploadInProgress = signal(false);
  protected editProductId = signal<string | null>(null);

  ngOnInit() {
    this.storefrontService.getCategories().subscribe((cats) => this.categories.set(cats));

    const productId = this.route.snapshot.queryParamMap.get('productId');

    this.shopService.getMyShop().subscribe({
      next: (s) => {
        this.shopId.set(s.id);
        if (productId) {
          this.isEdit.set(true);
          this.editProductId.set(productId);
          this.loadProductForEdit(s.id, productId);
        }
      },
      error: () => this.router.navigate(['/my-shop'])
    });
  }

  private loadProductForEdit(shopId: string, productId: string) {
    // No single-product read endpoint exists — fetch the shop's list and find
    // it there. Category/subcategory are not part of that DTO, so they are
    // left for the artist to re-select rather than guessed at.
    this.productService.getShopProducts(shopId, { pageSize: 100 }).subscribe({
      next: (res) => {
        const p = res.items.find((x) => x.id === productId);
        if (!p) return;
        this.form = {
          ...this.form,
          title: p.title,
          description: p.description ?? '',
          price: p.price
        };
        const primary = p.images.find((i) => i.isPrimary) ?? p.images[0];
        if (primary) this.form.imageUrl = primary.url;
      },
      error: () => {}
    });
  }

  getImageUrl(): string {
    return resolveImageUrl(this.form.imageUrl);
  }

  onFileChange(ev: Event) {
    const input = ev.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.uploadInProgress.set(true);
    this.filesService.upload(file, 'Product').subscribe({
      next: (res) => {
        this.form.imageRelativePath = res.relativePath;
        this.form.imageUrl = res.relativePath;
        this.uploadInProgress.set(false);
      },
      error: (err) => {
        this.uploadInProgress.set(false);
        alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل رفع الصورة.' : 'Failed to upload image.'));
      }
    });
  }

  submit() {
    const shopId = this.shopId();
    if (!shopId) return;

    if (!this.form.categoryId || !this.form.subCategoryId) {
      alert(this.lang() === 'ar' ? 'التصنيف والتصنيف الفرعي مطلوبان.' : 'Category and SubCategory are required.');
      return;
    }

    this.loading.set(true);

    if (this.isEdit() && this.editProductId()) {
      this.productService.updateProductMainInfo(this.editProductId()!, shopId, {
        title: this.form.title,
        categoryId: this.form.categoryId,
        subCategoryId: this.form.subCategoryId,
        images: this.form.imageRelativePath
          ? [{ url: this.form.imageRelativePath, altText: this.form.title, isPrimary: true, sortOrder: 0 }]
          : []
      }).subscribe({
        next: () => {
          this.loading.set(false);
          alert(this.lang() === 'ar' ? 'تم تحديث المنتج بنجاح!' : 'Product updated successfully!');
          this.router.navigate(['/my-shop']);
        },
        error: (err) => {
          this.loading.set(false);
          alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل تحديث المنتج.' : 'Failed to update product.'));
        }
      });
    } else {
      if (!this.form.imageRelativePath) {
        this.loading.set(false);
        alert(this.lang() === 'ar' ? 'الرجاء رفع صورة للمنتج.' : 'Please upload a product image.');
        return;
      }

      this.productService.createProduct({
        shopId,
        categoryId: this.form.categoryId,
        subCategoryId: this.form.subCategoryId,
        title: this.form.title,
        description: this.form.description,
        price: this.form.price,
        images: [{ relativePath: this.form.imageRelativePath, altText: this.form.title, isPrimary: true, sortOrder: 0 }]
      }).subscribe({
        next: () => {
          this.loading.set(false);
          this.router.navigate(['/my-shop']);
        },
        error: (err) => {
          this.loading.set(false);
          alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل إضافة المنتج.' : 'Failed to add product.'));
        }
      });
    }
  }

  cancel() {
    this.router.navigate(['/my-shop']);
  }

  deleteProduct() {
    if (!this.editProductId()) return;
    if (!confirm(this.lang() === 'ar' ? 'هل أنت متأكد من حذف هذا المنتج؟' : 'Are you sure you want to delete this product?')) return;

    this.loading.set(true);
    this.productService.deleteProduct(this.editProductId()!).subscribe({
      next: () => {
        this.loading.set(false);
        alert(this.lang() === 'ar' ? 'تم حذف المنتج بنجاح!' : 'Product deleted successfully!');
        this.router.navigate(['/my-shop']);
      },
      error: (err) => {
        this.loading.set(false);
        alert(apiErrorMessage(err, this.lang() === 'ar' ? 'فشل حذف المنتج.' : 'Failed to delete product.'));
      }
    });
  }
}
