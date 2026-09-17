import { Component, inject, computed } from '@angular/core';
import { I18nService } from '../../../core/i18n/i18n.service';
import { TPipe } from '../../../core/i18n/t.pipe';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [TPipe],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.css'
})
export class FooterComponent {
  readonly i18n = inject(I18nService);
  lang = computed(() => this.i18n.lang());
  protected readonly year = new Date().getFullYear();
}
