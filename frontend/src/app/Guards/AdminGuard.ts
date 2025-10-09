import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import {UserService} from '../services/UserService';


@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(private userService: UserService, private router: Router) {}

  canActivate(): boolean {
    // pobieramy synchronnie usera z BehaviorSubject
    const user = this.userService.currentUser;
    if (user?.role === 'Admin') {
      return true;
    }
    this.router.navigate(['/']);
    return false;
  }
}
