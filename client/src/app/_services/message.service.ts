import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Message } from '../_models/message';
import { PaginatedResult } from '../_models/pagination';
import {
  addPaginationHeaders,
  setPaginatedResultSignal,
} from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;

  private http = inject(HttpClient);
  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);

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

  postMessage(username: string, message: string) {
    return this.http.post<Message>(`${this.baseUrl}messages`, {
      receiverUsername: username,
      content: message,
    });
  }

  deleteMessageSvc(id: number) {
    return this.http.delete(`${this.baseUrl}messages/${id}`);
  }
}
