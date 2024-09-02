import { Component, computed, inject, input, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../_services/likes.service';
import { PresenceService } from '../../_services/presence.service';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
})
export class MemberCardComponent implements OnInit {
  private likesService = inject(LikesService);
  private presenceService = inject(PresenceService);

  shortIntro: string = '';
  fullIntro: string = '';
  isExpanded = false;
  memberInfo = input.required<Member>();

  //Computed Signal
  hasLiked = computed(() =>
    this.likesService.likedUserIds().includes(this.memberInfo().id)
  );

  isOnlineNow = computed(() =>
    this.presenceService.onlineUsers().includes(this.memberInfo().username)
  );

  ngOnInit(): void {
    this.shortIntro = this.memberInfo().introduction?.substring(0, 35);
    this.fullIntro = this.memberInfo().introduction;
  }

  toggleIntro() {
    this.isExpanded = !this.isExpanded;
  }

  toggleLike() {
    this.likesService.toggleLikeSvc(this.memberInfo().id).subscribe({
      next: () => {
        if (this.hasLiked()) {
          this.likesService.likedUserIds.update((ids) =>
            ids.filter((x) => x != this.memberInfo().id)
          );
          window.location.reload();
        } else {
          this.likesService.likedUserIds.update((ids) => [
            ...ids,
            this.memberInfo().id,
          ]);
        }
      },
    });
  }
}
