import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  OnInit,
  inject,
  viewChild,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TodoService } from './todo.service';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './todo-list.component.html',
  styleUrl: './todo-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodoListComponent implements OnInit {
  private service = inject(TodoService);
  private input =
    viewChild.required<ElementRef<HTMLInputElement>>('newTodoInput');

  readonly todos = this.service.todos;
  readonly loading = this.service.loading;
  readonly error = this.service.error;

  draft = '';

  ngOnInit(): void {
    this.service.load();
  }

  async submit(): Promise<void> {
    const value = this.draft;
    this.draft = '';
    await this.service.add(value);
    this.input().nativeElement.focus();
  }

  toggle(id: string, isDone: boolean): void {
    this.service.toggle(id, isDone);
  }

  remove(id: string): void {
    this.service.remove(id);
  }
}
