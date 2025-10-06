import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrls: ['../auth.css']
})
export class RegisterComponent {
  username: string = '';
  email: string = '';
  password: string = '';
  message: string = '';

  private apiUrl = 'http://localhost:5187/api/auth/register';
  // Twój endpoint backendu

  constructor(private http: HttpClient) {}

  register() {
    const payload = {
      username: this.username,
      email: this.email,
      password: this.password
    };

    this.http.post<{ message: string }>(this.apiUrl, payload).subscribe({
      next: (res) => {
        this.message = res.message; // teraz wyświetli dokładny komunikat z backendu
        this.username = '';
        this.email = '';
        this.password = '';
      },
      error: (err) => {
        console.error(err);
        this.message = err.status === 400 ? err.error : 'Błąd rejestracji. Sprawdź dane.';
      }
    });
  }
}
