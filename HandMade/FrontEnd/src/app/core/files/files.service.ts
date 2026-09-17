import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api/api.tokens';
import { UploadTarget } from '../api/api.models';

export interface UploadResponse {
  relativePath: string;
  absoluteUrl: string;
}

const ALLOWED_EXTENSIONS = ['.jpg', '.jpeg', '.png', '.webp'];
const MAX_FILE_SIZE_BYTES = 5 * 1024 * 1024; // 5 MB

export class FileValidationError extends Error {}

@Injectable({ providedIn: 'root' })
export class FilesService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  /** Client-side pre-check so bad files fail fast with a clear message before the round trip. */
  validate(file: File): void {
    const ext = file.name.slice(file.name.lastIndexOf('.')).toLowerCase();
    if (!ALLOWED_EXTENSIONS.includes(ext)) {
      throw new FileValidationError('Only .jpg, .jpeg, .png and .webp images are allowed.');
    }
    if (file.size > MAX_FILE_SIZE_BYTES) {
      throw new FileValidationError('Images must be 5 MB or smaller.');
    }
  }

  upload(file: File, target: UploadTarget = 'Product'): Observable<UploadResponse> {
    this.validate(file);
    const fd = new FormData();
    fd.append('File', file);
    const params = new HttpParams().set('target', target);
    return this.http.post<UploadResponse>(`${this.apiBaseUrl}/api/Files/upload`, fd, { params });
  }
}
