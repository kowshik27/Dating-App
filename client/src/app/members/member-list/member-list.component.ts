import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { MemberCardComponent } from '../member-card/member-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [
    MemberCardComponent,
    PaginationModule,
    FormsModule,
    ButtonsModule,
    NgClass,
  ],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  membersService = inject(MembersService);

  genderList = [
    { value: 'male', display: 'Male' },
    { value: 'female', display: 'Female' },
  ];

  ngOnInit(): void {
    if (!this.membersService.paginatedResult()) {
      this.loadMembers();
    }
  }

  loadMembers() {
    this.membersService.getMembers();
  }

  pageChanged(event: any) {
    if (this.membersService.userParams().pageNumber !== event.page) {
      this.membersService.userParams().pageNumber = event.page;
      this.loadMembers();
    }
  }

  resetFilters() {
    this.membersService.resetFilters();
    this.loadMembers();
  }
}
