import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, model, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParam';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  private accountService = inject(AccountService);

  baseUrl = environment.apiUrl;

  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  user = this.accountService.currentUser();
  userParams = signal<UserParams>(new UserParams(this.user));

  paginatedResultCache = new Map();

  resetFilters() {
    this.userParams.set(new UserParams(this.user));
  }

  getMembers() {
    var mapKey = Object.values(this.userParams()).join('-');

    const response = this.paginatedResultCache.get(mapKey);

    if (response) return this.setPaginatedResultSignal(response);

    let params = this.addPaginationHeaders(
      this.userParams().pageNumber,
      this.userParams().pageSize
    );
    params = params.append('minAge', this.userParams().minAge);
    params = params.append('maxAge', this.userParams().maxAge);
    params = params.append('gender', this.userParams().gender);
    params = params.append('orderBy', this.userParams().orderBy);

    return this.http
      .get<Member[]>(this.baseUrl + 'users', { observe: 'response', params })
      .subscribe({
        next: (response) => {
          this.setPaginatedResultSignal(response);
          this.paginatedResultCache.set(mapKey, response);
        },
      });
  }

  private addPaginationHeaders(pageNumber?: number, pageSize?: number) {
    let params = new HttpParams();
    if (pageNumber && pageSize) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }
    return params;
  }

  private setPaginatedResultSignal(response: HttpResponse<Member[]>) {
    this.paginatedResult.set({
      items: response.body as Member[],
      pagination: JSON.parse(response.headers.get('Pagination')!),
    });
  }

  getMember(username: string) {
    const member: Member = [...this.paginatedResultCache.values()]
      .reduce((arr, elem) => arr.concat(elem.body), [])
      .find((m: Member) => m.username === username);

    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http
      .put(this.baseUrl + 'users/', member)
      .pipe
      // tap(() => {
      //   this.members.update((members) =>
      //     members.map((m) => (m.username === member.username ? member : m))
      //   );
      // })
      ();
  }

  updateProfilePhoto(photo: Photo) {
    return this.http.put(
      this.baseUrl + 'users/set-profile-photo/' + photo.id,
      {}
    );
    // .pipe(
    // tap(() => {
    //   this.members.update((members) => {
    //     return members.map((m) => {
    //       // Make Note - Differ (return stat)
    //       if (m.photos.includes(photo)) m.photoUrl = photo.url;
    //       return m;
    //     });
    //   });
    // })
    // );
  }

  deletePhotoSvc(photo: Photo) {
    return this.http.delete(this.baseUrl + 'users/deLetePhoto/' + photo.id, {});
    // .pipe(
    //   tap(() => {
    //     this.members.update((members) => {
    //       return members.map((m) => {
    //         // Make Note - Differ (return stat)
    //         if (m.photos.includes(photo)) {
    //           m.photos = m.photos.filter((p) => p.id !== photo.id);
    //         }
    //         return m;
    //       });
    //     });
    //   })
    // );
  }
}
