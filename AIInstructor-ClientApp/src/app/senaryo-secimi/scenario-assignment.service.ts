import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../enviroments/environment';
import { Observable } from 'rxjs';

export interface AssignedScenario {
  id: string;
  senaryoId: string;
  ogrenciId: string;
  baslamaTarihi?: string;
  bitisTarihi?: string;
  puan?: number;
  badge?: string;
}

@Injectable({ providedIn: 'root' })
export class ScenarioAssignmentService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl.replace(/\/?ui$/i, '/api');

  getAssignmentsForStudent(ogrenciId: string): Observable<AssignedScenario[]> {
    return this.http.get<AssignedScenario[]>(`${this.baseUrl}/ogrenci/${ogrenciId}/senaryolar`);
  }

}
