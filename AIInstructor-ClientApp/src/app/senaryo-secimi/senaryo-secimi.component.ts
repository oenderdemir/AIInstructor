import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { ScenarioAssignmentService, AssignedScenario } from './scenario-assignment.service';

@Component({
  selector: 'app-senaryo-secimi',
  standalone: true,
  imports: [CommonModule, FormsModule, CardModule, InputTextModule, ButtonModule, TableModule],
  templateUrl: './senaryo-secimi.component.html',
  styleUrls: ['./senaryo-secimi.component.scss']
})
export class SenaryoSecimiComponent {
  ogrenciId = '';
  loading = false;
  assignments: AssignedScenario[] = [];

  constructor(private service: ScenarioAssignmentService, private router: Router) {}

  loadAssignments(): void {
    if (!this.ogrenciId) {
      return;
    }

    this.loading = true;
    this.service.getAssignmentsForStudent(this.ogrenciId).subscribe({
      next: (data) => {
        this.assignments = data ?? [];
        this.loading = false;
      },
      error: () => {
        this.assignments = [];
        this.loading = false;
      }
    });
  }

  startScenario(assignment: AssignedScenario): void {
    this.router.navigate(['/senaryo-calistir', assignment.id]);
  }
}
