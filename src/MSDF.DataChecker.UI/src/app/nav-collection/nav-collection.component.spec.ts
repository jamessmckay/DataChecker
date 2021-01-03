import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NavCollectionComponent } from './nav-collection.component';

describe('NavCollectionComponent', () => {
  let component: NavCollectionComponent;
  let fixture: ComponentFixture<NavCollectionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [NavCollectionComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NavCollectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
