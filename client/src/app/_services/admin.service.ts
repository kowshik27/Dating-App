import { inject, Injectable } from '@angular/core';
import { MembersService } from './members.service';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl;

  private memberService = inject(MembersService);
  private http = inject(HttpClient);

  getUserRoles() {
    return this.http.get(`${this.baseUrl}admin/user-roles`);
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post<string[]>(
      `${this.baseUrl}admin/edit-roles/${username}/?roles=${roles}`,
      {}
    );
  }
}
