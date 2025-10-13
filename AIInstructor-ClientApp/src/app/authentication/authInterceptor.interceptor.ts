// app/authentication/auth.interceptor.ts
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

function isExpiredServerSide(err: HttpErrorResponse): boolean {
  const wa = err.headers?.get('WWW-Authenticate') || err.headers?.get('Www-Authenticate') || '';
  const waLc = wa?.toLowerCase?.() ?? '';
  const bodyText = typeof err.error === 'string' ? err.error : JSON.stringify(err.error ?? {});
  const bodyLc = bodyText.toLowerCase();
  return (
    (waLc.includes('invalid_token') && waLc.includes('expired')) ||
    bodyLc.includes('token expired') || bodyLc.includes('expired')
  );
}

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  const url = req.url.toLowerCase();
  const isAuthCall = url.includes('/ui/auth/login') || url.includes('/refresh');

  // token/extoken oku
  const token = localStorage.getItem('token') || localStorage.getItem('extoken') || '';

  // Sadece token varsa ve login/refresh değilse header ekle
  const authReq = (!isAuthCall && token)
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // fetch adapter'da bazı 401'lar status:0 olarak gelebilir
      const unauthorized = error.status === 401 || error.status === 0;

      if (!isAuthCall && unauthorized && isExpiredServerSide(error)) {
        localStorage.removeItem('token');
        localStorage.removeItem('extoken');
        router.navigate(['/login']);
      }

      return throwError(() => error);
    })
  );
};
