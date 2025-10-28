import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../enviroments/environment';
import { Observable, tap } from 'rxjs';
import { jwtDecode } from "jwt-decode";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private tokenKey = 'token';
  private exTokenKey='extoken'
  isAuthenticated = signal(!!localStorage.getItem(this.tokenKey)); // Signal ile reaktif auth kontrolü
  private inactivityTimeout: any;
  userRoles = signal<string[]>([]);
  authChanged = signal<boolean>(this.isAuthenticated());

  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;


  constructor(private router: Router) {
    this.addActivityListeners();
    if (this.isAuthenticated()) {
      this.userRoles.set(this.getUserRoles());
    }
  }

  login(username: string, password: string) : Observable<any> {

    return this.http.post(this.baseUrl+"/Auth/login",{"kullaniciAdi":username,"parola":password}).pipe(
      tap((response: any) => {
        if (response?.authToken) {
          localStorage.setItem(this.tokenKey, response.authToken);
          this.userRoles.set(this.getUserRoles());
        }

        this.isAuthenticated.set(true);
        this.resetInactivityTimer();

        this.authChanged.set(true);
      }
    ));

}


changePassword(currentPassword: string, newPassword: string, newPassword2:string) : Observable<any> {

  return this.http.post(this.baseUrl+"/Auth/changePassword",{"currentPassword":currentPassword,"newPassword":newPassword,"newPassword2":newPassword2});
}


logout() {
  localStorage.removeItem(this.tokenKey);
  localStorage.removeItem(this.exTokenKey);
  this.isAuthenticated.set(false);
  this.userRoles.set([]); 
  clearTimeout(this.inactivityTimeout); 
  this.authChanged.set(false);


  setTimeout(() => {
      this.router.navigate(['/login']);
  }, 100);
}


  checkAuth(): boolean {
    return this.isAuthenticated();
  }

  resetInactivityTimer() {
    if (!this.isAuthenticated()) return; // Kullanıcı giriş yapmadıysa süreyi sıfırlama

    if (this.inactivityTimeout) {
      clearTimeout(this.inactivityTimeout);
    }
    this.inactivityTimeout = setTimeout(() => {
      this.logout();
      alert('Uzun süre işlem yapılmadığı için oturum sonlandırıldı.');
    }, 10  * 60 * 1000); // 10 dakika işlem yapılmazsa çıkış yap
  }

  private addActivityListeners() {
    ['mousemove', 'keydown', 'click'].forEach(event => {
      document.addEventListener(event, () => {
        if (this.isAuthenticated()) {
          this.resetInactivityTimer(); 
        }
      });
    });
  }

  getUserPermissions(): string[] {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      return decodedToken["permission"] || [];
    }
    return [];
  }

  getUserRoles(): string[] {
    const token = localStorage.getItem(this.tokenKey);
    if (!token) {
      return [];
    }

    const decodedToken: any = jwtDecode(token);
    const roles = decodedToken['role'];
    if (Array.isArray(roles)) {
      return roles;
    }

    if (typeof roles === 'string') {
      return [roles];
    }

    return [];
  }

  hasRole(role: string): boolean {
    const roles = this.getUserRoles();
    return roles.includes(role);
  }

  hasAnyRole(roles: string[]): boolean {
    const userRoles = this.getUserRoles();
    return roles.some(role => userRoles.includes(role));
  }
}
