import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  members = signal<Member[]>([]);

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users').subscribe({
      next: (membersResponse) => {
        this.members.set(membersResponse);
      },
    });
  }

  getMember(username: string) {
    const member = this.members().find((x) => x.username === username);
    if (member != undefined) {
      return of(member);
    }
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users/', member).pipe(
      tap(() => {
        this.members.update((members) =>
          members.map((m) => (m.username === member.username ? member : m))
        );
      })
    );
  }

  updateProfilePhoto(photo: Photo) {
    return this.http
      .put(this.baseUrl + 'users/set-profile-photo/' + photo.id, {})
      .pipe(
        tap(() => {
          this.members.update((members) => {
            return members.map((m) => {
              // Make Note - Differ (return stat)
              if (m.photos.includes(photo)) m.photoUrl = photo.url;
              return m;
            });
          });
        })
      );
  }

  deletePhotoSvc(photo: Photo) {
    return this.http
      .delete(this.baseUrl + 'users/deLetePhoto/' + photo.id, {})
      .pipe(
        tap(() => {
          this.members.update((members) => {
            return members.map((m) => {
              // Make Note - Differ (return stat)
              if (m.photos.includes(photo)) {
                m.photos = m.photos.filter((p) => p.id !== photo.id);
              }
              return m;
            });
          });
        })
      );
  }
}
