import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import {
  addPaginationHeaders,
  setPaginatedResultSignal,
} from './paginationHelper';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  likedUserIds = signal<number[]>([]);
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);

  predicate = 'liked';

  toggleLikeSvc(targetUserId: number) {
    return this.http.post(`${this.baseUrl}likes/${targetUserId}`, {});
  }

  getLikedUsers(predicate: string, pageNumber: number, pageSize: number) {
    let params = addPaginationHeaders(pageNumber, pageSize);

    params = params.append('predicate', predicate);

    return this.http
      .get<Member[]>(`${this.baseUrl}likes`, {
        observe: 'response',
        params,
      })
      .subscribe({
        next: (response) => {
          setPaginatedResultSignal(response, this.paginatedResult);
        },
      });
  }

  getLikedUsersId() {
    return this.http.get<number[]>(`${this.baseUrl}likes/list`).subscribe({
      next: (response) => this.likedUserIds.set(response),
    });
  }
}
