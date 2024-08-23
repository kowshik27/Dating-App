import { Component, inject, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from '../_models/message';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [
    ButtonsModule,
    FormsModule,
    TimeagoModule,
    RouterLink,
    PaginationModule,
  ],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.css',
})
export default class MessagesComponent implements OnInit {
  messageService = inject(MessageService);
  container = 'Inbox ';
  pageNumber = 1;
  pageSize = 2;
  isOutbox: boolean = this.container === 'Outbox';

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.messageService.getMessages(
      this.pageNumber,
      this.pageSize,
      this.container
    );
  }

  deleteMessage(id: number) {
    this.messageService.deleteMessageSvc(id).subscribe({
      next: (_) => {
        this.messageService.paginatedResult.update((x) => {
          if (x && x.items) {
            x.items.splice(
              x.items.findIndex((m) => m.id === id),
              1
            );
            return x;
          }
          return x;
        });
      },
    });
  }

  getRouterLink(message: Message) {
    if (this.container === 'Outbox')
      return `/members/${message.receiverUsername}`;
    else return `/members/${message.senderUsername}`;
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.pageNumber) {
      this.pageNumber = event.pageNumber;
      this.loadMessages();
    }
  }
}
