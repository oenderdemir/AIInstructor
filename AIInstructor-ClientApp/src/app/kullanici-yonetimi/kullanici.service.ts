import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { KullaniciModel } from './kullanici.model';
import { environment } from '../../enviroments/environment';

@Injectable({
  providedIn: 'root'
})
export class KullaniciService {
  private apiUrl = environment.apiUrl+'/Kullanici'; // API endpoint'in. İstersen environment.ts içinden de çekebilirsin.

  constructor(private http: HttpClient) {}

  // Kullanıcıları listele
  getAllKullanicilar(): Observable<KullaniciModel[]> {
    return this.http.get<KullaniciModel[]>(this.apiUrl);
  }

  // Yeni kullanıcı oluştur
  createKullanici(kullanici: KullaniciModel): Observable<KullaniciModel> {
    return this.http.post<KullaniciModel>(this.apiUrl, kullanici);
  }

  // Varolan kullanıcıyı güncelle
  updateKullanici(kullanici: KullaniciModel): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${kullanici.id}`, kullanici);
  }

  // Kullanıcıyı sil
  deleteKullanici(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Tek bir kullanıcı getir (ID'ye göre) - İstersen
  getKullaniciById(id: string): Observable<KullaniciModel> {
    return this.http.get<KullaniciModel>(`${this.apiUrl}/${id}`);
  }
}
