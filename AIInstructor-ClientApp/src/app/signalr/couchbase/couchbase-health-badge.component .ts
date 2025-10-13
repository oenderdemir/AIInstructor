// src/app/shared/components/couchbase-health-badge/couchbase-health-badge.component.ts
import { Component, OnDestroy, OnInit, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { Subscription } from 'rxjs';
import { SignalRService } from '../signal-r.service';
import { CbHealthModel } from './cb-health.model';


@Component({
  selector: 'app-couchbase-health-badge',
  standalone: true,
  imports: [CommonModule, TagModule, TooltipModule],
  template: `
  <p-tag
    [value]="text()"
    [severity]="severity()"
    icon="pi pi-database"
    [pTooltip]="tooltip()"
    tooltipPosition="bottom">
  </p-tag>
  `,
})
export class CouchbaseHealthBadgeComponent implements OnInit, OnDestroy {
  private sub?: Subscription;
  private data = signal<CbHealthModel | null>(null);

  constructor(private sr: SignalRService) {}

  ngOnInit(): void {
    this.sr.connect();
    this.sub = this.sr.healthChanges().subscribe(h => this.data.set(h));
  }
  ngOnDestroy(): void { this.sub?.unsubscribe(); }

  text = computed(() => this.data()?.healthy ? 'Couchbase: ONLINE' : 'Couchbase: OFFLINE');
  severity = computed(() => this.data()?.healthy ? 'success' : 'danger');
  tooltip = computed(() => {
    const h = this.data();
    if (!h) return 'Durum bekleniyor…';
    const t = new Date(h.at).toLocaleString();
    return h.healthy ? `Online @ ${t}` : `Offline @ ${t}${h.lastError ? '\n' + h.lastError : ''}`;
  });
}
