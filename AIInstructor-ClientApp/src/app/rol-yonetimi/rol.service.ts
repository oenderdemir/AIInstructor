import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../enviroments/environment';
import { RolModel } from './rol.model';


@Injectable({
  providedIn: 'root'
})
export class RolService {
  private apiUrl = environment.apiUrl+'/Rol'; // Buraya gerçek API endpointini yaz

  constructor(private http: HttpClient) {}

  getAllRols(): Observable<RolModel[]> {
    return this.http.get<RolModel[]>(this.apiUrl);
  }

  getAllViewRols(): Observable<RolModel[]> {
    return this.http.get<RolModel[]>(this.apiUrl+"/ViewRolleriniGetir");
  }
  getRol(id: number): Observable<RolModel> {
    return this.http.get<RolModel>(`${this.apiUrl}/${id}`);
  }

  createRol(Rol: RolModel): Observable<RolModel> {
    return this.http.post<RolModel>(this.apiUrl, Rol);
  }

  updateRol(Rol: RolModel): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${Rol.id}`, Rol);
  }

  deleteRol(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
