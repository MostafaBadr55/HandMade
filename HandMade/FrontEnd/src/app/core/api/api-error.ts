import { HttpErrorResponse } from '@angular/common/http';
import { ApiProblem, ValidationProblem } from './api.models';

/**
 * Normalises the API's two different error shapes (domain ProblemDetails vs
 * ASP.NET's automatic ValidationProblemDetails) plus network/auth failures
 * into one display string.
 */
export function apiErrorMessage(err: unknown, fallback = 'Something went wrong. Please try again.'): string {
  if (!(err instanceof HttpErrorResponse)) return fallback;

  if (err.status === 0) return 'Could not reach the server. Check your connection and try again.';

  const body = err.error;

  if (body && typeof body === 'object' && 'errors' in body) {
    const validation = body as ValidationProblem;
    const messages = Object.values(validation.errors ?? {}).flat();
    if (messages.length) return messages.join(' ');
    return validation.title ?? fallback;
  }

  if (body && typeof body === 'object' && 'errorCode' in body) {
    const problem = body as ApiProblem;
    return problem.detail || problem.title || fallback;
  }

  if (err.status === 401) return 'Your session has expired. Please log in again.';
  if (err.status === 403) return 'You are not allowed to do that.';
  if (err.status === 404) return 'Not found.';

  return fallback;
}
