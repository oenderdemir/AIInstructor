import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RolYonetimiComponent } from './rol-yonetimi.component';

describe('RolYonetimiComponent', () => {
  let component: RolYonetimiComponent;
  let fixture: ComponentFixture<RolYonetimiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RolYonetimiComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RolYonetimiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
