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
import { KullaniciModel } from './kullanici.model';
import { enumKullaniciStatus } from './kullanici-status.enum';
import { KullaniciService } from './kullanici.service';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { TabViewModule } from 'primeng/tabview';
import { KullaniciGrupService } from '../kullanici-grup-yonetimi/kullanici-grup.service';
import { ChipModule } from 'primeng/chip';

@Component({
  selector: 'app-kullanici-yonetimi',
  standalone: true,
  templateUrl: './kullanici-yonetimi.component.html',
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    DropdownModule,
    DividerModule,
    TabViewModule,
    MultiSelectModule,
    ConfirmDialog,
    ToastModule,
    ChipModule
  ],
  providers: [ConfirmationService, MessageService]
})
export class KullaniciYonetimiComponent implements OnInit {
  kullanicilar: KullaniciModel[] = [];
  selectedKullanici: KullaniciModel = this.getEmptyKullanici();
  displayDialog: boolean = false;
  isEditMode: boolean = false;
  allKullaniciGruplar: { id: string, ad: string }[] = [];
  selectedKullaniciGrupIds: string[] = [];
  seciliKullanicininRolleri: string[] = [];

  constructor(
    private kullaniciService: KullaniciService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    public authService: AuthService,
    private kullaniciGrupService: KullaniciGrupService
  ) {}

  ngOnInit(): void {
    this.loadKullanicilar();
    this.loadAllKullaniciGruplar();
  }

  loadKullanicilar() {
    this.kullaniciService.getAllKullanicilar().subscribe(response => {
      this.kullanicilar = response;
    });
  }

  loadAllKullaniciGruplar() {
    this.kullaniciGrupService.getAllKullaniciGrup().subscribe(response => {
      this.allKullaniciGruplar = response.map(grup => ({
        id: grup.id!,
        ad: grup.ad
      }));
    });
  }

  getEmptyKullanici(): KullaniciModel {
    return {
      ad: '',
      soyad: '',
      email: '',
      kullaniciAdi: '',
      status: enumKullaniciStatus.SifreDegistirmeli,
      kullaniciGruplar: []
    };
  }

  openNew() {
    this.selectedKullanici = this.getEmptyKullanici();
    this.selectedKullaniciGrupIds = [];
    this.seciliKullanicininRolleri = [];
    this.isEditMode = false;
    this.displayDialog = true;
  }

  editKullanici(kullanici: KullaniciModel) {
    this.selectedKullanici = { ...kullanici };
    this.selectedKullaniciGrupIds = kullanici.kullaniciGruplar?.map(g => g.id!) ?? [];
    this.seciliKullanicininRolleri = kullanici.kullaniciGruplar
      ?.flatMap(grup => grup.roller?.map(rol => rol.domain + "." + rol.ad) ?? [])
      .filter((value, index, self) => self.indexOf(value) === index) ?? [];
    this.isEditMode = true;
    this.displayDialog = true;
  }

  saveKullanici() {
    this.selectedKullanici.kullaniciGruplar = this.selectedKullaniciGrupIds.map(id => ({ id, ad: '' }));

    if (this.isEditMode) {
      this.kullaniciService.updateKullanici(this.selectedKullanici).subscribe(() => {
        this.loadKullanicilar();
        this.displayDialog = false;
        this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Kullanıcı güncellendi' });
      });
    } else {
      this.kullaniciService.createKullanici(this.selectedKullanici).subscribe(() => {
        this.loadKullanicilar();
        this.displayDialog = false;
        this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Yeni kullanıcı eklendi' });
      });
    }
  }

  deleteKullanici(kullanici: KullaniciModel) {
    this.confirmationService.confirm({
      message: 'Bu kullanıcıyı silmek istediğinize emin misiniz?',
      header: 'Onayla',
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-secondary',
      acceptLabel: 'Evet',
      rejectLabel: 'Hayır',
      accept: () => {
        if (!kullanici.id) return;
        this.kullaniciService.deleteKullanici(kullanici.id).subscribe(() => {
          this.loadKullanicilar();
          this.messageService.add({
            severity: 'success',
            summary: 'Başarılı',
            detail: 'Kullanıcı başarıyla silindi'
          });
        });
      }
    });
  }
}
