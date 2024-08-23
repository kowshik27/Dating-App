import {
  Component,
  inject,
  input,
  OnInit,
  output,
  ViewChild,
} from '@angular/core';
import { MessageService } from '../../_services/message.service';
import { Message } from '../../_models/message';
import { TimeagoModule } from 'ngx-timeago';
import { NgClass } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [TimeagoModule, NgClass, FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css',
})
export class MemberMessagesComponent {
  private messageService = inject(MessageService);

  receiverUsername = input.required<string>();
  messages = input.required<Message[]>();
  messageBody = '';
  @ViewChild('messageForm')
  messageForm!: NgForm;

  newMessage = output<Message>();

  sendNewMessage() {
    console.log('Button CLicked !!!!!!!!!!');
    this.messageService
      .postMessage(this.receiverUsername(), this.messageBody)
      .subscribe({
        next: (message) => {
          this.newMessage.emit(message);
          this.messageForm?.reset();
        },
      });
  }
}
