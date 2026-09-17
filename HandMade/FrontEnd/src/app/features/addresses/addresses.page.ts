import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AddressService } from '../../core/address/address.service';
import { Address } from '../../core/address/address.models';
import { I18nService } from '../../core/i18n/i18n.service';
import { apiErrorMessage } from '../../core/api/api-error';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './addresses.page.html',
  styleUrls: ['./addresses.page.css']
})
export class AddressesPage implements OnInit {
  private readonly addressService = inject(AddressService);
  readonly i18n = inject(I18nService);
  lang = computed(() => this.i18n.lang());

  loading = signal(true);
  addresses = signal<Address[]>([]);
  error = signal<string | null>(null);

  showForm = signal(false);
  editingId = signal<string | null>(null);
  label = signal('');
  detailedAddress = signal('');
  isDefault = signal(false);
  saving = signal(false);

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading.set(true);
    this.addressService.getMyAddresses().subscribe({
      next: (addrs) => {
        this.addresses.set(addrs);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err));
        this.loading.set(false);
      }
    });
  }

  openNew() {
    this.editingId.set(null);
    this.label.set('');
    this.detailedAddress.set('');
    this.isDefault.set(false);
    this.showForm.set(true);
  }

  openEdit(a: Address) {
    this.editingId.set(a.id);
    this.label.set(a.label);
    this.detailedAddress.set(a.detailedAddress);
    this.isDefault.set(a.isDefault);
    this.showForm.set(true);
  }

  save() {
    if (!this.label().trim() || !this.detailedAddress().trim()) return;
    this.saving.set(true);

    const editing = this.editingId();
    const request$ = editing
      ? this.addressService.updateAddress(editing, { label: this.label(), detailedAddress: this.detailedAddress() })
      : this.addressService.createAddress({
          label: this.label(),
          detailedAddress: this.detailedAddress(),
          isDefault: this.isDefault()
        });

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.showForm.set(false);
        this.load();
      },
      error: (err) => {
        this.saving.set(false);
        alert(apiErrorMessage(err));
      }
    });
  }

  setDefault(a: Address) {
    this.addressService.setDefault(a.id).subscribe({
      next: () => this.load(),
      error: (err) => alert(apiErrorMessage(err))
    });
  }

  remove(a: Address) {
    if (!confirm(this.lang() === 'ar' ? 'هل تريد حذف هذا العنوان؟' : 'Delete this address?')) return;
    this.addressService.deleteAddress(a.id).subscribe({
      next: () => this.load(),
      error: (err) => alert(apiErrorMessage(err))
    });
  }
}
