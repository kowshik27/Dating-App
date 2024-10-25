import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Message } from '../_models/message';
import { PaginatedResult } from '../_models/pagination';
import {
  addPaginationHeaders,
  setPaginatedResultSignal,
} from './paginationHelper';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { User } from '../_models/user';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubsUrl;

  private http = inject(HttpClient);
  hubConnection?: HubConnection;

  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);
  messageThread = signal<Message[]>([]);

  createHubConnection(user: User, otherUsername: string) {
    console.log('Creating Conn for Msg - createHub fnx working');
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}message?user=${otherUsername}`, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch((e) => console.log(`Error Occured in Presence SiganlR - ${e}`));

    this.hubConnection.on('ReceiveMessageThread', (messagesRes) => {
      console.log('Response\t\t--', messagesRes.result?.content);
      this.messageThread.set(messagesRes.result);
    });

    this.hubConnection.on('NewMessage', (messageRes) => {
      this.messageThread.update((messages) => [...messages, messageRes]);
    });

    // Optimizations for not sending the whole thread everytime
    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if (group.connections.some((x) => x.username === otherUsername)) {
        this.messageThread.update((messages) => {
          console.log('Messages Before: --------\n', messages);
          messages.forEach((message) => {
            if (!message.messageReadAt) {
              message.messageReadAt = new Date(Date.now());
            }
          });
          console.log('\nMessages After: --------\n', messages);
          return messages;
        });
      }
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection
        .stop()
        .catch((e) =>
          console.log(`Error Occured in Presence Stopping SiganlR- ${e}`)
        );
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = addPaginationHeaders(pageNumber, pageSize);
    params = params.append('container', container);

    return this.http
      .get<Message[]>(`${this.baseUrl}messages/all/`, {
        observe: 'response',
        params,
      })
      .subscribe({
        next: (response) =>
          setPaginatedResultSignal(response, this.paginatedResult),
      });
  }

  getMessageThread(senderUsername: string) {
    return this.http.get<Message[]>(
      `${this.baseUrl}messages/thread/${senderUsername}`
    );
  }

  async postMessage(username: string, content: string) {
    return this.hubConnection?.invoke('SendMessage', {
      receiverUsername: username,
      content,
    });
  }

  deleteMessageSvc(id: number) {
    return this.http.delete(`${this.baseUrl}messages/${id}`);
  }
}
