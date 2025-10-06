import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {tap} from 'rxjs';
import {UserService} from './UserService';

interface LoginResponse {
  token: string;
  username: string;
  role: string;
}
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5187/api/Auth';

  constructor(private http: HttpClient, private userService: UserService) {}

  register(user: { username: string; email: string; password: string }) {
    return this.http.post<{ message: string }>(
      'http://localhost:5187/api/auth/register',
      user
    );
  }
  login(user: { username: string; password: string }) {
    return this.http.post<LoginResponse>(
      'http://localhost:5187/api/auth/login',
      user
    ).pipe(
      tap(res => {
        localStorage.setItem('token', res.token);
        localStorage.setItem('username', res.username);
        localStorage.setItem('role', res.role);
        // ustaw usera w UserService
        this.userService.loadUser(res.username, res.role); // <- teraz nagłówek zobaczy username
      })
    );
  }






}
