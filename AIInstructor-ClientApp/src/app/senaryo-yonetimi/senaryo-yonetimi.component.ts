import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ScenarioSessionService, StartScenarioResponse, ScenarioTurnResponse } from './scenario-session.service';

@Component({
  selector: 'app-senaryo-yonetimi',
  standalone: true,
  imports: [CommonModule, CardModule, InputTextModule, ButtonModule, FormsModule, TextareaModule, ToastModule],
  providers: [MessageService],
  templateUrl: './senaryo-yonetimi.component.html',
  styleUrls: ['./senaryo-yonetimi.component.scss']
})
export class SenaryoYonetimiComponent {
  ogrenciAdi = '';
  senaryoBasladi = false;
  ogrenciMesaji = '';
  mesajlar: { rol: string, icerik: string }[] = [];
  sessionId: string | null = null;

  evaluation: any = null;

  constructor(
    private scenarioService: ScenarioSessionService,
    private messageService: MessageService
  ) {}

  startScenario() {
    if (!this.ogrenciAdi.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Uyarı', detail: 'Öğrenci adını giriniz.' });
      return;
    }

    const scenarioId = 'hotel-checkin-basic'; // 🔹 backend’deki senaryo id
    this.scenarioService.startScenario(scenarioId, this.ogrenciAdi).subscribe({
      next: (res: StartScenarioResponse) => {
        this.senaryoBasladi = true;
        this.sessionId = res.sessionId;
        this.mesajlar = [
          { rol: 'sistem', icerik: 'Senaryo başlatıldı.' },
          { rol: 'asistan', icerik: res.tutorMessage }
        ];
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Hata', detail: 'Senaryo başlatılamadı.' });
        console.error(err);
      }
    });
  }

  ileri() {
    if (!this.ogrenciMesaji.trim() || !this.sessionId) return;

    const userMessage = this.ogrenciMesaji;
    this.mesajlar.push({ rol: 'öğrenci', icerik: userMessage });
    this.ogrenciMesaji = '';

    this.scenarioService.submitMessage(this.sessionId, this.ogrenciAdi, userMessage).subscribe({
      next: (res: ScenarioTurnResponse) => {
        this.mesajlar.push({ rol: 'asistan', icerik: res.tutorMessage });

        if (res.isCompleted) {
          this.evaluation = res.evaluation; // 🎯 evaluation sonucu alındı
          this.messageService.add({ severity: 'success', summary: 'Senaryo Tamamlandı', detail: 'Değerlendirme hazır.' });
        }
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Hata', detail: 'Mesaj gönderilemedi.' });
        console.error(err);
      }
    });
  }
}
