import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TodoListComponent } from './todo-list.component';
import { TodoService } from './todo.service';

describe('TodoListComponent', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoListComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('loads todos on init and renders them', () => {
    const fixture = TestBed.createComponent(TodoListComponent);
    fixture.detectChanges();

    const req = httpMock.expectOne('/api/todos');
    expect(req.request.method).toBe('GET');
    req.flush([
      { id: '1', title: 'Buy milk', isDone: false, createdAt: '2025-01-15T10:30:00Z' },
    ]);

    fixture.detectChanges();

    const titles = fixture.nativeElement.querySelectorAll('.title');
    expect(titles.length).toBe(1);
    expect(titles[0].textContent).toContain('Buy milk');
  });

  it('calls the service to add a todo when the form is submitted', () => {
    const fixture = TestBed.createComponent(TodoListComponent);
    fixture.detectChanges();
    httpMock.expectOne('/api/todos').flush([]);

    const component = fixture.componentInstance;
    component.draft = 'Walk dog';
    fixture.detectChanges();

    const form = fixture.nativeElement.querySelector('form');
    form.dispatchEvent(new Event('submit'));

    const req = httpMock.expectOne('/api/todos');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ title: 'Walk dog' });
    req.flush({ id: '2', title: 'Walk dog', isDone: false, createdAt: '2025-01-15T10:31:00Z' });
  });
});
