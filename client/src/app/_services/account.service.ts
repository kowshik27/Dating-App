import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl = 'https://localhost:5001/api/';
  currentUser = signal<User | null>(null);

  loginSvc(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  logoutSvc() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  registerSvc(registerModel: any) {
    return this.http
      .post<User>(this.baseUrl + 'account/register', registerModel)
      .pipe(
        map((user) => {
          if (user) {
            this.setCurrentUser(user);
          }
          return user;
        })
      );
  }

  setCurrentUser(user: User): void {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }
}
