import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SenaryoYonetimiComponent } from './senaryo-yonetimi.component';

describe('SenaryoYonetimiComponent', () => {
  let component: SenaryoYonetimiComponent;
  let fixture: ComponentFixture<SenaryoYonetimiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SenaryoYonetimiComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SenaryoYonetimiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
