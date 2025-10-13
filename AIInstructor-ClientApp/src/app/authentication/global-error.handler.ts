import { ErrorHandler, Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

function looksExpired(err: HttpErrorResponse): boolean {
  const wa = err.headers?.get('WWW-Authenticate') || err.headers?.get('Www-Authenticate') || '';
  const waLc = wa.toLowerCase();
  const body = typeof err.error === 'string' ? err.error : JSON.stringify(err.error ?? {});
  const bodyLc = body.toLowerCase();
  return (waLc.includes('invalid_token') && waLc.includes('expired')) ||
         bodyLc.includes('token expired') || bodyLc.includes('expired');
}

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private injector: Injector) {}

  handleError(err: unknown): void {
    if (err instanceof HttpErrorResponse) {
      const router = this.injector.get(Router);
      if ((err.status === 401 || err.status === 0) && looksExpired(err)) {
        localStorage.removeItem('token');
        router.navigate(['/login']);
        return; // swallow
      }
    }
    // İstersen burada logging servisine gönder
    // console.error(err);
  }
}
