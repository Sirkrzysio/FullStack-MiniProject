import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface User {
  id: number;
  username: string;
  email: string;
  role: string;
}

@Component({
  standalone: true,
  imports: [CommonModule],
  selector: 'app-admin',
  templateUrl: './admin.html',
  styleUrls: ['./admin.css']
})
export class AdminComponent implements OnInit {

  users: User[] = [];
  loading = true;
  error = '';

  private apiUrl = 'http://localhost:5187/api/admin';

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchUsers();
  }

  fetchUsers() {
    this.loading = true;
    this.http.get<User[]>(`${this.apiUrl}/users`).subscribe({
      next: (res) => {
        this.users = res;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Błąd podczas pobierania użytkowników';
        this.loading = false;
      }
    });
  }

  deleteUser(id: number) {
    if (!confirm('Czy na pewno chcesz usunąć tego użytkownika?')) return;

    this.http.delete(`${this.apiUrl}/users/${id}`).subscribe({
      next: () => {
        this.users = this.users.filter(u => u.id !== id);
      },
      error: () => alert('Nie udało się usunąć użytkownika')
    });
  }
}
