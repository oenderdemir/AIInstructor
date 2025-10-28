import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../enviroments/environment';
import { Observable } from 'rxjs';

export interface ConversationMessage {
  role: 'student' | 'ai';
  content: string;
}

export interface EvaluationResponse {
  success: boolean;
  hints: string[];
  badge?: string;
  puan?: number;
}

@Injectable({ providedIn: 'root' })
export class SenaryoCalistirService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl.replace(/\/?ui$/i, '/api');

  sendMessages(ogrenciSenaryoId: string, messages: string[]): Observable<EvaluationResponse> {
    return this.http.post<EvaluationResponse>(`${this.baseUrl}/ai-instructor/evaluate`, {
      ogrenciSenaryoId,
      messages
    });
  }
}
