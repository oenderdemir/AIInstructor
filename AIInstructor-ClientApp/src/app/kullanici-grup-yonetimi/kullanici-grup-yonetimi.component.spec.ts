import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KullaniciGrupYonetimiComponent } from './kullanici-grup-yonetimi.component';

describe('KullaniciGrupYonetimiComponent', () => {
  let component: KullaniciGrupYonetimiComponent;
  let fixture: ComponentFixture<KullaniciGrupYonetimiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KullaniciGrupYonetimiComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(KullaniciGrupYonetimiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
