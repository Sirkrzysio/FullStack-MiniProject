import {Component, OnInit} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TodoService} from '../../services/todo.service';
import {TodoItem} from '../../models/todo-item';
import {FilterTodosPipe} from '../../pipes/filter-todos.pipe';
import {FileUploadService} from '../../services/FileUploadService';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [CommonModule, FormsModule, FilterTodosPipe],
  templateUrl: './todo-list.html',
  styleUrl: './todo-list.css'
})
export class TodoListComponent implements OnInit {
  todos: TodoItem[] = [];
  newTodoTitle = '';
  filter: 'all' | 'active' | 'completed' = 'all';
  editingId: number | null = null;
  editingTitle = '';

  constructor(
    private todoService: TodoService,
    private fileService: FileUploadService
  ) {}

  ngOnInit() {
    this.loadTodos();
  }

  loadTodos() {
    this.todoService.getMyTodos().subscribe({
      next: todos => this.todos = todos,
      error: err => console.error('Błąd pobierania todos', err)
    });
  }

  addTodo() {
    const title = this.newTodoTitle.trim();
    if (!title) return;

    this.todoService.addTodo({ title, completed: false }).subscribe({
      next: () => {
        this.newTodoTitle = '';
        this.loadTodos();
      },
      error: err => console.error('Błąd dodawania todo', err)
    });
  }

  toggleCompleted(todo: TodoItem) {
    this.todoService.updateTodo(todo.id, { completed: !todo.completed }).subscribe({
      next: () => this.loadTodos(),
      error: err => console.error('Błąd aktualizacji todo', err)
    });
  }

  startEditing(todo: TodoItem) {
    this.editingId = todo.id;
    this.editingTitle = todo.title;
  }

  saveEditing(todo: TodoItem) {
    const title = this.editingTitle.trim();
    if (!title) return;

    this.todoService.updateTodo(todo.id, { title, completed: todo.completed }).subscribe({
      next: () => {
        this.editingId = null;
        this.editingTitle = '';
        this.loadTodos();
      },
      error: err => console.error('Błąd zapisu edycji', err)
    });
  }

  deleteTodo(id: number) {
    if (!confirm('Czy na pewno chcesz usunąć to zadanie?')) return;

    this.todoService.deleteTodo(id).subscribe({
      next: () => this.loadTodos(),
      error: err => console.error('Błąd usuwania todo', err)
    });
  }

  setFilter(type: 'all' | 'active' | 'completed') {
    this.filter = type;
  }

  importTodos(event: any) {
    const file = event.target.files[0];
    if (!file) return;

    this.fileService.uploadFile(file).subscribe({
      next: res => {
        alert(`${res.count} todo dodano z pliku!`);
        this.loadTodos();
      },
      error: err => {
        console.error('Błąd importu', err);
        alert('Nie udało się zaimportować pliku');
      }
    });
  }

  exportTodos() {
    const data = JSON.stringify(this.todos, null, 2);
    const blob = new Blob([data], { type: 'application/json' });
    const url = URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.href = url;
    a.download = 'todos.json';
    a.click();

    URL.revokeObjectURL(url);
  }

  clearAll() {
    if (!confirm('Czy na pewno chcesz wyczyścić wszystkie zadania?')) return;

    const ids = this.todos.map(t => t.id);
    const requests = ids.map(id => this.todoService.deleteTodo(id));

    // Subskrybujemy ostatni request i odświeżamy listę
    if (requests.length > 0) {
      requests[requests.length - 1].subscribe({
        next: () => this.loadTodos(),
        error: err => console.error('Błąd usuwania', err)
      });

      
      requests.slice(0, -1).forEach(req => req.subscribe({ error: err => console.error('Błąd usuwania', err) }));
    }
  }

}
