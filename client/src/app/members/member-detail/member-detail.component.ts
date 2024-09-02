import {
  Component,
  inject,
  OnDestroy,
  OnInit,
  ViewChild,
  viewChild,
} from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';
import { HubConnection, HubConnectionState } from '@microsoft/signalr';

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
export class MemberDetailComponent implements OnInit, OnDestroy {
  private messageService = inject(MessageService);
  private accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  presenceService = inject(PresenceService);

  @ViewChild('memDetailTabs', { static: true }) memDetailTabs?: TabsetComponent;
  activeTab?: TabDirective; // For knowing which tab is activated

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

    // Routing from Toastr and getting the Hub Connection
    this.route.paramMap.subscribe({
      next: (_) => this.onRouteParamChange(),
    });
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  // Routing from Toastr and getting the Hub Connection

  onRouteParamChange() {
    const user = this.accountService.currentUser();
    if (!user) return;
    if (
      this.messageService.hubConnection?.state ===
        HubConnectionState.Connected &&
      this.activeTab?.heading === 'Messages'
    ) {
      this.messageService.hubConnection
        .stop()
        .then(() =>
          this.messageService.createHubConnection(user, this.member.username)
        );
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { tab: this.activeTab.heading },
      queryParamsHandling: 'merge',
    });
    if (this.activeTab.heading === 'Messages' && this.member) {
      const otherUser = this.member.username;
      const user = this.accountService.currentUser();
      if (!user) return;
      console.log('MemDet OnTabActivated Working !!..');
      this.messageService.createHubConnection(user, otherUser);
      console.log(this.messageService.messageThread());
    } else {
      this.messageService.stopHubConnection();
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
}
