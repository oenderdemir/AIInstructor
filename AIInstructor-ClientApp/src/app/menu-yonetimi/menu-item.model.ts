import { RolModel } from "../rol-yonetimi/rol.model";

export interface MenuItemDto {
  id?: string | null;
  label?: string;
  icon?: string;
  routerLink?: string;
  queryParams?: string;
  parentId?: string | null;
  menuOrder: number;
  items?: MenuItemDto[] | null;
  roles?: RolModel[] | null;
}

  