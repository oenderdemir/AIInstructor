import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MenuYonetimiComponent } from './menu-yonetimi.component';

describe('MenuYonetimiComponent', () => {
  let component: MenuYonetimiComponent;
  let fixture: ComponentFixture<MenuYonetimiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MenuYonetimiComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MenuYonetimiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
