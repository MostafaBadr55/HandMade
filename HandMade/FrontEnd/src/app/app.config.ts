import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';

import { appRoutes } from './app.routes';
import { API_BASE_URL } from './core/api/api.tokens';
import { authTokenInterceptor } from './core/api/auth-token.interceptor';
import { authErrorInterceptor } from './core/api/auth-error.interceptor';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),

    provideRouter(
      appRoutes,
      withInMemoryScrolling({ scrollPositionRestoration: 'enabled', anchorScrolling: 'enabled' })
    ),

    provideHttpClient(withInterceptors([authTokenInterceptor, authErrorInterceptor])),

    // In dev this is '' so services emit relative /api/... paths that the dev
    // server proxy (proxy.conf.json) forwards to the backend; a production
    // build swaps in environment.production.ts via angular.json fileReplacements.
    { provide: API_BASE_URL, useValue: environment.apiBaseUrl }
  ]
};
