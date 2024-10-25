import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';
import { LikesService } from './likes.service';
import { PresenceService } from './presence.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  private likesService = inject(LikesService);
  private presenceService = inject(PresenceService);

  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);
  roles = computed(() => {
    const user = this.currentUser();
    if (user && user.token) {
      const role = JSON.parse(atob(user.token.split('.')[1])).role;

      return Array.isArray(role) ? role : [role];
    }
    return [];
  });

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
    this.presenceService.stopHubConnection();
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
    this.likesService.getLikedUsersId();
    this.presenceService.createHubConnection(user);
  }
}
