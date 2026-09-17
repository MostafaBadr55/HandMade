import { DOCUMENT } from '@angular/common';
import { Injectable, inject, signal } from '@angular/core';

import { DICTIONARY, TranslationKey } from './i18n.dictionary';
import { Lang } from './i18n.types';

const STORAGE_KEY = 'unlimited_lang';

@Injectable({ providedIn: 'root' })
export class I18nService {
  private readonly document = inject(DOCUMENT);

  private readonly _lang = signal<Lang>('ar');

  constructor() {
    const stored = (localStorage.getItem(STORAGE_KEY) as Lang | null) ?? null;
    const initial = stored === 'en' || stored === 'ar' ? stored : 'ar';
    this.setLang(initial);
  }

  lang(): Lang {
    return this._lang();
  }

  setLang(lang: Lang): void {
    this._lang.set(lang);
    localStorage.setItem(STORAGE_KEY, lang);

    const html = this.document.documentElement;
    html.lang = lang;
    html.dir = lang === 'ar' ? 'rtl' : 'ltr';
    html.dataset['lang'] = lang;
  }

  t(key: TranslationKey): string {
    return DICTIONARY[this.lang()][key] ?? key;
  }
}
