import { Injectable } from '@angular/core';
import { TodoItem } from '../models/todo-item';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private storageKey = 'todos';

  // Prywatna tablica w pamięci serwisu
  private todos: TodoItem[] = [];

  constructor() {

    const data = localStorage.getItem(this.storageKey);
    this.todos = data ? JSON.parse(data) : [];
  }

  // Pobierz listę zadań
  getTodos(): TodoItem[] {
    return this.todos;
  }

  // Zapisz listę do localStorage
  private saveTodos() {
    localStorage.setItem(this.storageKey, JSON.stringify(this.todos));
  }

  // Dodaj nowe zadanie
  addTodo(todo: TodoItem) {
    this.todos.push(todo);
    this.saveTodos();
  }

  // Usuń zadanie
  deleteTodo(id: number) {
    this.todos = this.todos.filter(t => t.id !== id);
    this.saveTodos();
  }

  // Przełącz completed
  toggleCompleted(id: number) {
    const todo = this.todos.find(t => t.id === id);
    if (todo) {
      todo.completed = !todo.completed;
      this.saveTodos();
    }
  }

  // Aktualizuj zadanie
  updateTodo(updatedTodo: TodoItem) {
    const index = this.todos.findIndex(t => t.id === updatedTodo.id);
    if (index > -1) {
      this.todos[index] = updatedTodo;
      this.saveTodos();
    }
  }
  clearTodos() {
    this.todos = [];
    localStorage.removeItem(this.storageKey);
  }

}
