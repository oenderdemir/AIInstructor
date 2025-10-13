import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { KullaniciGrupService } from './kullanici-grup.service';
import { KullaniciGrupModel } from './kullanici-grup.model';
import { RolService } from '../rol-yonetimi/rol.service';
import { DividerModule } from 'primeng/divider';
import { DropdownModule } from 'primeng/dropdown';
import { TabViewModule } from 'primeng/tabview';
import { TreeTableModule } from 'primeng/treetable';
import { AuthService } from '../authentication/auth.service';

@Component({
  selector: 'app-kullanici-grup-yonetimi',
  standalone: true,
  templateUrl: './kullanici-grup-yonetimi.component.html',
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    DropdownModule,
    TreeTableModule,
    DividerModule,
    TabViewModule,
    MultiSelectModule,
    ConfirmDialog,
    ToastModule
  ],
  providers: [ConfirmationService, MessageService]
})
export class KullaniciGrupYonetimiComponent implements OnInit {
  kullaniciGruplari: KullaniciGrupModel[] = [];
  selectedGrup: KullaniciGrupModel = this.getEmptyGrup();
  displayDialog: boolean = false;
  isEditMode: boolean = false;

  allRoles: { id: number, name: string }[] = [];
  selectedRoles: number[] = [];

  constructor(
    private grupService: KullaniciGrupService,
    private rolService: RolService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadGroups();
    this.loadAllRoles();
  }

  loadGroups() {
    this.grupService.getAllKullaniciGrup().subscribe(response => {
      this.kullaniciGruplari = response;
    });
  }

  loadAllRoles() {
    this.rolService.getAllRols().subscribe(response => {
      this.allRoles = response.map(role => ({
        id: role.id,
        name: `${role.domain}.${role.ad}`
      }));
    });
  }

  getEmptyGrup(): KullaniciGrupModel {
    return {
      id: null,
      ad: '',
      roller: []
    };
  }

  openNew() {
    this.selectedGrup = this.getEmptyGrup();
    this.selectedRoles = [];
    this.isEditMode = false;
    this.displayDialog = true;
  }

  editGrup(grup: KullaniciGrupModel) {
    this.selectedGrup = { ...grup };
    this.selectedRoles = grup.roller?.map(r => r.id) ?? [];
    this.isEditMode = true;
    this.displayDialog = true;
  }

  saveGrup() {
    const payload: KullaniciGrupModel = {
      id: this.selectedGrup.id,
      ad: this.selectedGrup.ad,
      roller: this.selectedRoles.map(id => ({ id: id })) as any[]
    };

    if (this.isEditMode) {
      this.grupService.updateKullaniciGrup(payload).subscribe(() => {
        this.loadGroups();
        this.displayDialog = false;
        this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Grup güncellendi' });
      });
    } else {
      this.grupService.createKullaniciGrup(payload).subscribe((result) => {
        this.kullaniciGruplari.push(result);
        this.displayDialog = false;
        this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Yeni grup eklendi' });
      });
    }
  }

  deleteGrup(grup: KullaniciGrupModel) {
    this.confirmationService.confirm({
      message: 'Bu grubu silmek istediğinize emin misiniz?',
      header: 'Onayla',
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-secondary',
      acceptLabel: 'Evet',
      rejectLabel: 'Hayır',
      accept: () => {
        if (!grup.id) return;
        this.grupService.deleteKullaniciGrup(grup.id).subscribe(() => {
          this.loadGroups();
          this.messageService.add({
            severity: 'success',
            summary: 'Başarılı',
            detail: 'Grup başarıyla silindi'
          });
        });
      }
    });
  }
}
