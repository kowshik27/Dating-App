import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { LikesService } from '../_services/likes.service';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { MemberCardComponent } from '../members/member-card/member-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-lists',
  standalone: true,
  imports: [ButtonsModule, FormsModule, MemberCardComponent, PaginationModule],
  templateUrl: './lists.component.html',
  styleUrl: './lists.component.css',
})
export class ListsComponent implements OnInit, OnDestroy {
  public likesService = inject(LikesService);
  predicate: string = 'liked';
  pageNumber = 1;
  pageSize = 4;

  ngOnInit(): void {
    this.loadLikes();
  }

  ngOnDestroy(): void {
    this.likesService.paginatedResult.set(null);
  }

  getTitle() {
    switch (this.predicate) {
      case 'liked':
        return 'Members You Liked';
      case 'likedBy':
        return 'Members Liked You';
      default:
        return 'Mutual Likes';
    }
  }

  loadLikes() {
    this.likesService.getLikedUsers(
      this.predicate,
      this.pageNumber,
      this.pageSize
    );
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.likesService.getLikedUsers(
        this.predicate,
        this.pageNumber,
        this.pageSize
      );
    }
  }
}
