import { Component, inject, input, ViewChild } from '@angular/core';
import { MessageService } from '../../_services/message.service';
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
  messageService = inject(MessageService);

  receiverUsername = input.required<string>();
  messageBody = '';
  @ViewChild('messageForm')
  messageForm!: NgForm;

  sendNewMessage() {
    this.messageService
      .postMessage(this.receiverUsername(), this.messageBody)
      .then(() => {
        this.messageForm?.reset();
      })
      .catch((err) => console.log('Error in sending msg - ', err));
  }
}
