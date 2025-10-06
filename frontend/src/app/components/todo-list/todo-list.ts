import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TodoService} from '../../services/todo.service';
import {TodoItem} from '../../models/todo-item';
import {FilterTodosPipe} from '../../pipes/filter-todos.pipe';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [CommonModule, FormsModule, FilterTodosPipe],
  templateUrl: './todo-list.html',
  styleUrl: './todo-list.css'
})
export class TodoListComponent {
  todos: TodoItem[] = [];
  newTodoTitle = '';

  // Wstrzykujemy serwis
  constructor(private todoService: TodoService) {
    this.loadTodos();
  }

  // Pobierz listę z serwisu/localStorage
  loadTodos() {
    this.todos = this.todoService.getTodos();
  }

  // Dodaj nowe zadanie
  addTodo() {
    const title = this.newTodoTitle.trim();
    if (!title) return;

    const newTodo: TodoItem = {
      id: Date.now(),
      title,
      completed: false
    };

    this.todoService.addTodo(newTodo);
    this.newTodoTitle = '';
    this.loadTodos();
  }

  // Toggle completed
  toggleCompleted(todo: TodoItem) {
    this.todoService.toggleCompleted(todo.id);
    this.loadTodos();
  }

  // Usuń zadanie
  deleteTodo(id: number) {
    this.todoService.deleteTodo(id);
    this.loadTodos();
  }

  filter: 'all' | 'active' | 'completed' = 'all';

  setFilter(type: 'all' | 'active' | 'completed') {
    this.filter = type;
  }

  editingId: number | null = null;
  editingTitle: string = '';

  startEditing(todo: TodoItem) {
    this.editingId = todo.id;
    this.editingTitle = todo.title;
  }

  saveEditing(todo: TodoItem) {
    const title = this.editingTitle.trim();
    if (title) {
      todo.title = title;
      this.todoService.updateTodo(todo);
    }
    this.editingId = null;
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

  importTodos(event: any) {
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (e: any) => {
      try {
        const imported: TodoItem[] = JSON.parse(e.target.result);
        // można dodać walidację
        imported.forEach(todo => this.todoService.addTodo(todo));
        this.todos = this.todoService.getTodos();
      } catch (err) {
        console.error('Błąd przy imporcie', err);
        alert('Nieprawidłowy plik JSON');
      }
    };
    reader.readAsText(file);
  }
  clearAll() {
    if (!confirm('Czy na pewno chcesz wyczyścić wszystkie zadania?')) return;

    this.todos = [];
    this.todoService.clearTodos();
  }


}
