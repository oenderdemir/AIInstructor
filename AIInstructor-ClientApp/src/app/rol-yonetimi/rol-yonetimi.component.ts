import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DividerModule } from 'primeng/divider';

import { AuthService } from '../authentication/auth.service';

import { RolService } from './rol.service';
import { RolModel } from './rol.model';

@Component({
  selector: 'app-rol-yonetimi',
  standalone: true,
  templateUrl: './rol-yonetimi.component.html',
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    DividerModule,
    ConfirmDialog,
    ToastModule
  ],
  providers: [ConfirmationService, MessageService]
})
export class RolYonetimiComponent implements OnInit {
  roller: RolModel[] = [];
  selectedRol: RolModel = this.getEmptyRol();
  displayDialog: boolean = false;
  isEditMode: boolean = false;

  constructor(
    private rolService: RolService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadRoller();
  }

  loadRoller() {
    this.rolService.getAllRols().subscribe(response => {
      this.roller = response;
    });
  }

  getEmptyRol(): RolModel {
    return {
      id: 0,
      ad: '',
      domain: ''
    };
  }

  openNew() {
    this.selectedRol = this.getEmptyRol();
    this.isEditMode = false;
    this.displayDialog = true;
  }

  editRol(rol: RolModel) {
    this.selectedRol = { ...rol };
    this.isEditMode = true;
    this.displayDialog = true;
  }

  saveRol() {
    if (this.isEditMode) {
      this.rolService.updateRol(this.selectedRol).subscribe(() => {
        this.loadRoller();
        this.displayDialog = false;
        this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Rol güncellendi' });
      });
    } else {
      this.rolService.createRol(this.selectedRol).subscribe((result) => {
        this.roller.push(result);
        this.displayDialog = false;
        this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Yeni rol eklendi' });
      });
    }
  }

  deleteRol(rol: RolModel) {
    this.confirmationService.confirm({
      message: 'Bu rolü silmek istediğinize emin misiniz?',
      header: 'Onayla',
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-secondary',
      acceptLabel: 'Evet',
      rejectLabel: 'Hayır',
      accept: () => {
        this.rolService.deleteRol(rol.id).subscribe(() => {
          this.loadRoller();
          this.messageService.add({
            severity: 'success',
            summary: 'Başarılı',
            detail: 'Rol başarıyla silindi'
          });
        });
      }
    });
  }
}
