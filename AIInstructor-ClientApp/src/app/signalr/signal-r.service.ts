// src/app/core/services/signalr.service.ts
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';

import { environment } from '../../enviroments/environment';
import { CbHealthModel } from './couchbase/cb-health.model';
// Eğer JWT varsa: import { AuthService } from '../../authentication/auth.service';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hub?: signalR.HubConnection;
  private connection$ = new BehaviorSubject<boolean>(false);
  private health$ = new BehaviorSubject<CbHealthModel | null>(null);

  // constructor(private auth: AuthService) {}

  connect(): void {
    if (this.hub) return;

    const base = environment.apiUrl.replace(/\/$/, '');
    const url = `${base}/hubs/system`;

    this.hub = new signalR.HubConnectionBuilder()
      .withUrl(url, {
        withCredentials: true,
        // accessTokenFactory: () => this.auth.getToken() ?? ''
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .build();

    this.hub.on('couchbaseHealth', (payload: CbHealthModel) => {
      this.health$.next(payload);
    });

    this.hub.onclose(() => this.connection$.next(false));
    this.hub.onreconnecting(() => this.connection$.next(false));
    this.hub.onreconnected(() => this.connection$.next(true));

    this.hub.start()
      .then(() => this.connection$.next(true))
      .catch(err => console.error('SignalR start failed', err));
  }

  healthChanges(): Observable<CbHealthModel | null> { return this.health$.asObservable(); }
  connectionChanges(): Observable<boolean> { return this.connection$.asObservable(); }
}
