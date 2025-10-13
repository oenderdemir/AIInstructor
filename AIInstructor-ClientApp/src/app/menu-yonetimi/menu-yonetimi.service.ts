import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../enviroments/environment';
import { MenuItemDto } from './menu-item.model';

@Injectable({
  providedIn: 'root'
})
export class MenuYonetimiService {
  private apiUrl = environment.apiUrl+'/MenuItem'; // Buraya gerçek API endpointini yaz

  constructor(private http: HttpClient) {}

  getAllMenuItems(): Observable<MenuItemDto[]> {
    return this.http.get<MenuItemDto[]>(this.apiUrl);
  }

  getMenuItem(id: string): Observable<MenuItemDto> {
    return this.http.get<MenuItemDto>(`${this.apiUrl}/${id}`);
  }

  createMenuItem(menuItem: MenuItemDto): Observable<MenuItemDto> {
    return this.http.post<MenuItemDto>(this.apiUrl, menuItem);
  }

  updateMenuItem(menuItem: MenuItemDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${menuItem.id}`, menuItem);
  }

  deleteMenuItem(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
