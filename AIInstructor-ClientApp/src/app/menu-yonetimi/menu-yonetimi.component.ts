import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { TreeTableModule } from 'primeng/treetable';
import { DividerModule } from 'primeng/divider';
import { TabViewModule } from 'primeng/tabview';
import { MultiSelectModule } from 'primeng/multiselect';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService, TreeNode } from 'primeng/api';

import { MenuItemDto } from './menu-item.model';
import { MenuYonetimiService } from './menu-yonetimi.service';
import { RolService } from '../rol-yonetimi/rol.service';
import { RolModel } from '../rol-yonetimi/rol.model';
import { MenuService } from '../menu/menu.service';
import { AuthService } from '../authentication/auth.service';

@Component({
  selector: 'app-menu-yonetimi',
  standalone: true,
  templateUrl: './menu-yonetimi.component.html',
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
export class MenuYonetimiComponent implements OnInit {
  menuItems: MenuItemDto[] = [];
  selectedMenuItem: MenuItemDto = this.getEmptyMenuItem();
  displayDialog = false;
  isEditMode = false;
  parentMenus: MenuItemDto[] = [];
  selectedParentId: string = "";
  treeMenuItems: TreeNode[] = [];
  allRoles: { id: number; name: string }[] = [];
  selectedRoles: number[] = [];

  constructor(
    private menuYonetimService: MenuYonetimiService,
    private rolService: RolService,
    private menuService: MenuService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadMenuItems();
    this.loadAllRoles();
  }

  getEmptyMenuItem(): MenuItemDto {
    return {
      id: null,
      label: '',
      icon: '',
      routerLink: '',
      queryParams: '',
      parentId: null,
      menuOrder: 0,
      roles: [],
      items: []
    };
  }

  loadAllRoles() {
    this.rolService.getAllViewRols().subscribe(response => {
      this.allRoles = response.map(role => ({
        id: role.id,
        name: `${role.domain}.${role.ad}`
      }));
    });
  }

  buildTree(items: MenuItemDto[]): TreeNode[] {
    const map = new Map<string, TreeNode>();

    items.forEach(item => {
      if (!item.id) return;
      map.set(item.id, {
        key: item.id,
        data: item,
        children: []
      });
    });

    const tree: TreeNode[] = [];

    items.forEach(item => {
      if (!item.id) return;

      const currentNode = map.get(item.id);
      if (!currentNode) return;

      if (item.parentId) {
        const parent = map.get(item.parentId);
        if (parent) {
          parent.children?.push(currentNode);
        }
      } else {
        tree.push(currentNode);
      }
    });

    return tree;
  }

  hasChild(node: TreeNode): boolean {
    return !!(node.children && node.children.length > 0);
  }

  loadMenuItems() {
    this.menuYonetimService.getAllMenuItems().subscribe({
      next: items => {
        this.menuItems = items;
        this.treeMenuItems = this.buildTree(items);
        this.parentMenus = [
          {
            id: '',
            label: '/',
            icon: '',
            routerLink: '',
            queryParams: '',
            parentId: null,
            menuOrder: -10,
            roles: [],
            items: []
          },
          ...items.filter(x => !x.parentId)
        ];
      },
      error: err => console.error('Menü yüklenemedi', err)
    });
  }

  openNew() {
    this.selectedMenuItem = this.getEmptyMenuItem();
    this.selectedParentId = "";
    this.isEditMode = false;
    this.displayDialog = true;
  }

  openNewChild(parentId: string) {
    this.selectedMenuItem = this.getEmptyMenuItem();
    this.selectedParentId = parentId;
    this.isEditMode = false;
    this.displayDialog = true;
  }

  editMenuItem(menuItem: MenuItemDto) {
    this.selectedMenuItem = { ...menuItem };
    this.selectedParentId = menuItem.parentId ?? "";
    this.selectedRoles = (menuItem.roles || []).map(role => role.id);
    this.isEditMode = true;
    this.displayDialog = true;
  }

  saveMenuItem() {
    this.selectedMenuItem.parentId = this.selectedParentId === "" ? null : this.selectedParentId;

    const payload: MenuItemDto = {
      id: this.isEditMode ? this.selectedMenuItem.id : null,
      label: this.selectedMenuItem.label,
      icon: this.selectedMenuItem.icon,
      routerLink: this.selectedMenuItem.routerLink,
      queryParams: this.selectedMenuItem.queryParams,
      parentId: this.selectedMenuItem.parentId,
      menuOrder: this.selectedMenuItem.menuOrder,
      roles: this.selectedRoles.map(id => ({ id })) as RolModel[],
      items: []
    };

    const finalize = () => {
      this.loadMenuItems();
      this.displayDialog = false;
      this.menuService.loadMenu();
    };

    if (this.isEditMode) {
      this.menuYonetimService.updateMenuItem(payload).subscribe(finalize);
    } else {
      this.menuYonetimService.createMenuItem(payload).subscribe(item => {
        this.menuItems.push(item);
        this.treeMenuItems = this.buildTree(this.menuItems);
        this.displayDialog = false;
        this.menuService.loadMenu();
      });
    }
  }

  deleteMenuItem(menuItem: MenuItemDto) {
    this.confirmationService.confirm({
      message: 'Bu menüyü silmek istediğinize emin misiniz?',
      header: 'Onayla',
      icon: 'pi pi-exclamation-triangle',
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-secondary',
      acceptLabel: 'Evet',
      rejectLabel: 'Hayır',
      accept: () => {
        if (!menuItem.id) return;
        this.menuYonetimService.deleteMenuItem(menuItem.id).subscribe({
          next: () => {
            this.menuItems = this.menuItems.filter(x => x.id !== menuItem.id);
            this.treeMenuItems = this.buildTree(this.menuItems);
            this.menuService.loadMenu();
            this.messageService.add({ severity: 'success', summary: 'Başarılı', detail: 'Menü başarıyla silindi' });
          },
          error: err => {
            console.error('Menü silinemedi', err);
            this.messageService.add({ severity: 'error', summary: 'Hata', detail: 'Menü silinemedi' });
          }
        });
      },
      reject: () => {
        this.messageService.add({ severity: 'info', summary: 'İptal Edildi', detail: 'Silme işlemi iptal edildi' });
      }
    });
  }
}
