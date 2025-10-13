import { KullaniciGrupModel } from "../kullanici-grup-yonetimi/kullanici-grup.model";
import { enumKullaniciStatus } from "./kullanici-status.enum";

export interface KullaniciModel {
  id?: string | null;
  kullaniciAdi: string;
  tcno?: string;
  ad?: string;
  soyad?: string;
  email?: string;
  avatarPath?: string;
  status: enumKullaniciStatus;
  kullaniciGruplar: KullaniciGrupModel[];
}
