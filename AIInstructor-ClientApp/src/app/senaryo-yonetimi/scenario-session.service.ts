import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface StartScenarioResponse {
  sessionId: string;
  summary: any;
  tutorMessage: string;
  currentTurn: number;
  maxTurns: number;
}

export interface ScenarioTurnResponse {
  sessionId: string;
  tutorMessage: string;
  currentTurn: number;
  maxTurns: number;
  isCompleted: boolean;
  evaluation?: any;
  gamification?: any;
}

@Injectable({
  providedIn: 'root'
})
export class ScenarioSessionService {
  private apiUrl = 'https://localhost:7215/ui/TrainingScenarios'; // 🔹 kendi backend URL’ine göre güncelle

  constructor(private http: HttpClient) {}

   startScenario(scenarioId: string, studentId: string): Observable<StartScenarioResponse> {
    const body = { studentId };
    return this.http.post<StartScenarioResponse>(`${this.apiUrl}/${scenarioId}/sessions`, body);
  }
   /** 🟡 Öğrenci mesajını gönder */
  submitMessage(sessionId: string, studentId: string, message: string): Observable<ScenarioTurnResponse> {
    const body = { studentId, message };
    return this.http.post<ScenarioTurnResponse>(
      `${this.apiUrl}/sessions/${sessionId}/student-message`,
      body
    );
  }

  completeScenario(sessionId: string, studentId: string): Observable<ScenarioTurnResponse> {
    return this.http.post<ScenarioTurnResponse>(`${this.apiUrl}/complete`, { sessionId, studentId });
  }
}
