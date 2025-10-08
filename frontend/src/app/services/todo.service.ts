import { Injectable } from '@angular/core';
import { TodoItem } from '../models/todo-item';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private apiUrl = 'http://localhost:5187/api/Todo';

  constructor(private http: HttpClient) {}

  private getHeaders() {
    const token = localStorage.getItem('token'); // JWT z logowania
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  // GET /api/Todo/mytodos
  getMyTodos(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(`${this.apiUrl}/mytodos`, { headers: this.getHeaders() });
  }

  // POST /api/Todo/mytodos
  addTodo(todo: Partial<TodoItem>): Observable<TodoItem> {
    return this.http.post<TodoItem>(`${this.apiUrl}/mytodos`, todo, { headers: this.getHeaders() });
  }

  // PUT /api/Todo/{id}
  updateTodo(id: number, updated: Partial<TodoItem>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, updated, { headers: this.getHeaders() });
  }

  // DELETE /api/Todo/{id}
  deleteTodo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}
