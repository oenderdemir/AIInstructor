import { RolModel } from "../rol-yonetimi/rol.model";


export interface KullaniciGrupModel {
  id?: string|null;
  ad: string;
  roller?: RolModel[];
}