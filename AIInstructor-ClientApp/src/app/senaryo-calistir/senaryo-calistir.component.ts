import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DialogModule } from 'primeng/dialog';
import { InputTextarea } from 'primeng/inputtextarea';
import { SenaryoCalistirService } from './senaryo-calistir.service';
import { Subscription } from 'rxjs';

interface ChatMessage {
  sender: 'student' | 'ai';
  content: string;
}

@Component({
  selector: 'app-senaryo-calistir',
  standalone: true,
  imports: [CommonModule, FormsModule, CardModule, InputTextarea, ButtonModule, DialogModule],
  templateUrl: './senaryo-calistir.component.html',
  styleUrls: ['./senaryo-calistir.component.scss']
})
export class SenaryoCalistirComponent implements OnInit, OnDestroy {
  ogrenciSenaryoId = '';
  currentMessage = '';
  messages: ChatMessage[] = [];
  evaluationVisible = false;
  evaluationBadge?: string;
  evaluationScore?: number;
  hints: string[] = [];
  private subscription?: Subscription;

  constructor(private route: ActivatedRoute, private service: SenaryoCalistirService) {}

  ngOnInit(): void {
    this.subscription = this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.ogrenciSenaryoId = id;
      }
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  sendMessage(): void {
    if (!this.currentMessage.trim()) {
      return;
    }

    this.messages.push({ sender: 'student', content: this.currentMessage });
    const messageToSend = this.currentMessage;
    this.currentMessage = '';

    const studentMessages = this.messages.filter(m => m.sender === 'student').map(m => m.content);
    this.service.sendMessages(this.ogrenciSenaryoId, studentMessages).subscribe(response => {
      if (response?.hints?.length) {
        this.hints = response.hints;
        this.evaluationVisible = true;
      }
      if (response?.badge) {
        this.evaluationBadge = response.badge;
      }
      if (response?.puan !== undefined) {
        this.evaluationScore = response.puan;
      }

      this.messages.push({ sender: 'ai', content: `Değerlendirme gönderildi (${messageToSend.length} karakter).` });
    });
  }
}
