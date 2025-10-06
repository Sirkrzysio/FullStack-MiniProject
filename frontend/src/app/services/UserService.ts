import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';


export interface User {
  username: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private userSubject = new BehaviorSubject<User | null>(null);
  user$ = this.userSubject.asObservable();

  loadUser(username: string, role: string) {
    this.userSubject.next({ username, role });
  }

  clearUser() {
    this.userSubject.next(null);
  }
}

