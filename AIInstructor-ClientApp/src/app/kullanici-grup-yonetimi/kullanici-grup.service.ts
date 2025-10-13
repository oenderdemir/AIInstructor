import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { KullaniciGrupModel } from './kullanici-grup.model';
import { environment } from '../../enviroments/environment';

@Injectable({
  providedIn: 'root'
})
export class KullaniciGrupService {
  private apiUrl = environment.apiUrl+'/KullaniciGrup'; // Backend adresin

  constructor(private http: HttpClient) {}

  getAllKullaniciGrup(): Observable<KullaniciGrupModel[]> {
    return this.http.get<KullaniciGrupModel[]>(this.apiUrl);
  }

  createKullaniciGrup(grup: KullaniciGrupModel): Observable<KullaniciGrupModel> {
    return this.http.post<KullaniciGrupModel>(this.apiUrl, grup);
  }

  updateKullaniciGrup(grup: KullaniciGrupModel): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${grup.id}`, grup);
  }

  deleteKullaniciGrup(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
