export interface CbHealthModel {
  healthy: boolean;
  lastError?: string | null;
  at: string;         // ISO date
  scope?: string;
}