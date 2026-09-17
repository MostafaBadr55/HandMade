/**
 * Resolves an image value from the API into something an <img> can load.
 *
 * The API is inconsistent about absolute vs relative: ProductDetailsResponseVM
 * images[].url is absolute, ProductCardResponseVM.relativePath may be either,
 * and FileUploadResponseVM returns both relativePath and absoluteUrl. Prefer
 * relativePath on upload — absoluteUrl is built from the *incoming* request's
 * host, which behind the dev proxy is the backend origin, not the Angular one.
 */
export function resolveImageUrl(value?: string | null): string {
  if (!value || !value.trim()) return 'assets/placeholder.png';
  if (/^https?:\/\//i.test(value)) return value;
  return value.startsWith('/') ? value : `/${value}`;
}
