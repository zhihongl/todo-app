import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { TodoItem } from './todo.model';

@Injectable({ providedIn: 'root' })
export class TodoService {
  private http = inject(HttpClient);
  private apiUrl = '/api/todos';

  readonly todos = signal<TodoItem[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  async load(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const items = await firstValueFrom(this.http.get<TodoItem[]>(this.apiUrl));
      this.todos.set(items);
    } catch {
      this.error.set('Could not load todos.');
    } finally {
      this.loading.set(false);
    }
  }

  async add(title: string): Promise<void> {
    const trimmed = title.trim();
    if (!trimmed) return;
    try {
      const item = await firstValueFrom(
        this.http.post<TodoItem>(this.apiUrl, { title: trimmed }),
      );
      this.todos.update((list) => [...list, item]);
    } catch {
      this.error.set('Could not add todo.');
    }
  }

  async remove(id: string): Promise<void> {
    try {
      await firstValueFrom(this.http.delete<void>(`${this.apiUrl}/${id}`));
      this.todos.update((list) => list.filter((t) => t.id !== id));
    } catch {
      this.error.set('Could not delete todo.');
    }
  }

  async toggle(id: string, isDone: boolean): Promise<void> {
    try {
      const updated = await firstValueFrom(
        this.http.patch<TodoItem>(`${this.apiUrl}/${id}`, { isDone }),
      );
      this.todos.update((list) => list.map((t) => (t.id === id ? updated : t)));
    } catch {
      this.error.set('Could not update todo.');
    }
  }
}
