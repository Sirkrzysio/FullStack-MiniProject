import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService} from '../../../services/AuthService';
import {UserService} from '../../../services/UserService';
import {Router} from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrls: ['../auth.css']
})
export class LoginComponent {
  username = '';
  password = '';
  message = '';

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private router: Router
  ) {
  }

  login() {
    this.authService.login({username: this.username, password: this.password}).subscribe({
      next: (res) => {
        // zapis tokena i danych użytkownika
        localStorage.setItem('token', res.token);
        localStorage.setItem('username', res.username);
        localStorage.setItem('role', res.role);

        // ustawiamy użytkownika w UserService
        this.userService.loadUser(res.username, res.role);

        this.message = '✅ Zalogowano pomyślnie';
        this.username = '';
        this.password = '';

        // ukrycie komunikatu po 3 sekundach
        setTimeout(() => this.message = '', 3000);
      },
      error: () => {
        this.message = '❌ Błędne dane logowania';
        setTimeout(() => this.message = '', 3000);
      }
    });
  }
}


