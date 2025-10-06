import {Component, OnInit} from '@angular/core';
import {Router, RouterLink, RouterLinkActive} from '@angular/router';
import {RouterModule} from '@angular/router';
import {CommonModule} from '@angular/common';
import {User, UserService} from '../../services/UserService';

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    RouterLinkActive,
    RouterModule,
    CommonModule
  ],
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header {
  currentUser: User | null = null;

  constructor(private router: Router, public userService: UserService) {
    // subskrybujemy zmiany użytkownika tylko raz
    this.userService.user$.subscribe(user => {
      console.log('currentUser:', user); // logujemy dla debugowania
      this.currentUser = user;
    });
  }

  isLoggedIn(): boolean {
    return !!this.currentUser;
  }

  getUsername(): string {
    return this.currentUser?.username || '';
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    this.userService.clearUser();
    this.router.navigate(['/login']);
  }
}

