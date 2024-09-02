import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { ToastrService } from 'ngx-toastr';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { User } from '../_models/user';
import { take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubsUrl;
  onlineUsers = signal<string[]>([]);

  private toastr = inject(ToastrService);

  private hubConnection?: HubConnection;
  private router = inject(Router);

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}presence/`, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch((e) => console.log(`Error Occured in Presence SiganlR - ${e}`));

    this.hubConnection.on('UserIsOnline', (username) => {
      this.onlineUsers.update((users) => [...users, username]);
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      this.onlineUsers.update((users) => users.filter((x) => x !== username));
    });

    this.hubConnection.on('GetOnlineUsers', (usersRes) => {
      this.onlineUsers.set(usersRes);
    });

    // Notify Users abt New msgs
    this.hubConnection.on('NewMessageReceived', ({ username, nickname }) => {
      this.toastr
        .info(`New Message from ${nickname} \n Click to View`)
        .onTap.pipe(take(1)) // No worries abt unsubscribe
        .subscribe(() =>
          this.router.navigateByUrl(`/members/${username}?tab=Messages`)
        );
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection
        .stop()
        .catch((e) =>
          console.log(`Error Occured in Presence SiganlR Stopping- ${e}`)
        );
    }
  }
}
