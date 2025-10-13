import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KullaniciYonetimiComponent } from './kullanici-yonetimi.component';

describe('KullaniciYonetimiComponent', () => {
  let component: KullaniciYonetimiComponent;
  let fixture: ComponentFixture<KullaniciYonetimiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KullaniciYonetimiComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(KullaniciYonetimiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
