import { Component, inject, OnInit, ViewChild, viewChild } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [
    TabsModule,
    GalleryModule,
    TimeagoModule,
    DatePipe,
    MemberMessagesComponent,
  ],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent implements OnInit {
  private membersService = inject(MembersService);
  private messageService = inject(MessageService);
  private route = inject(ActivatedRoute);

  @ViewChild('memDetailTabs', { static: true }) memDetailTabs?: TabsetComponent;
  activeTab?: TabDirective; // For knowing which tab is activated
  messages: Message[] = [];

  member: Member = {} as Member;
  images: GalleryItem[] = [];

  ngOnInit(): void {
    this.route.data.subscribe({
      next: (data) => {
        this.member = data['member'];
        this.member &&
          this.member.photos.map((p) => {
            this.images.push(new ImageItem({ src: p.url, thumb: p.url }));
          });
      },
    });

    this.route.queryParams.subscribe({
      next: (params) => {
        params['tab'] && this.activateTab(params['tab']);
      },
    });
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (
      this.activeTab.heading === 'Messages' &&
      this.messages.length === 0 &&
      this.member
    ) {
      this.messageService.getMessageThread(this.member.username).subscribe({
        next: (msgsResponse) => (this.messages = msgsResponse),
      });
    }
  }

  activateTab(heading: string) {
    if (this.memDetailTabs) {
      const msgsTab = this.memDetailTabs.tabs.find(
        (x) => x.heading === heading
      );
      if (msgsTab) msgsTab.active = true;
    }
  }

  addNewMessage(event: Message) {
    this.messages.push(event);
  }
}
