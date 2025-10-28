import { Injectable, signal,computed, effect  } from '@angular/core';

import { AuthService } from '../authentication/auth.service';
import { MenuItem } from 'primeng/api';
import { MenuItemModel } from './menu-item.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../enviroments/environment';
import { MenuItemDto } from '../menu-yonetimi/menu-item.model';


@Injectable({
  providedIn: 'root'
})
export class MenuService {
  menuItems = signal<MenuItemModel[]>([]);
  userRoles = signal<string[]>([]);
  userPermissions = signal<string[]>([]);

  constructor(private authService: AuthService,private http: HttpClient) {
    //this.userRoles.set(this.authService.getUserPermissions());
    //this.loadMenu();

    // AuthService içindeki authChanged event'ini dinle
    effect(() => {
      if (this.authService.authChanged()) {
        this.userRoles.set(this.authService.getUserRoles());
        this.userPermissions.set(this.authService.getUserPermissions());
        this.loadMenu();
      }
    });
  }

  loadMenu() {
    this.http.get<MenuItemDto[]>(environment.apiUrl + "/MenuItem/MenuAgaciGetir").subscribe(response => {
      
      const mappedMenu: MenuItemModel[] = this.mapMenuItems(response);
  
      this.menuItems.set(mappedMenu); // Menü sistemine set ediyoruz
    });
  }
  
  // Yardımcı fonksiyon:
  private mapMenuItems(menuItems: MenuItemDto[] | null | undefined): MenuItemModel[] {
    if (!menuItems) {
      return [];
    }
  
    return menuItems.map(menuItem => ({
      label: menuItem.label,
      icon: menuItem.icon,
      routerLink: menuItem.routerLink,
      roles: menuItem.roles?.map(role => role.domain === 'SystemRole' ? role.ad : `${role.domain}.${role.ad}`) || [],
      items: menuItem.items ? this.mapMenuItems(menuItem.items) : undefined  // 👈 Alt menüler için recursive map
    }));
  }
  

  clearMenu() {
    this.menuItems.set([]); // Çıkış yapıldığında menüyü sıfırla
  }

  hasPermission(menuRoles?: string[]): boolean {
    if (this.userPermissions().includes('KullaniciTipi.Admin')) {
      return true; // Eğer kullanıcı "admin" ise her şeyi görebilir
    }

    if (!menuRoles || menuRoles.length === 0) {
      return true; // Eğer öğenin belirli bir rol gereksinimi yoksa herkese göster
    }
    const hasPermission = menuRoles.some(role => this.userPermissions().includes(role));
    if (hasPermission) {
      return true;
    }

    let hasRole=menuRoles.some(role => this.userRoles().includes(role));
      return hasRole; // Kullanıcının herhangi bir rolü eşleşiyorsa göster
  }

  getMenu = computed(() => {
    return this.menuItems().map(category => ({
        ...category,
        items: category.items?.filter(item => this.hasPermission(item.roles)) || []
    })).filter(category => 
        (category.items && category.items.length > 0) ||  // Alt öğesi varsa
        category.routerLink ||  // RouterLink varsa
        category.command // Command fonksiyonu varsa
    );
});


}
