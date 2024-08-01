import { Component, input, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
})
export class MemberCardComponent implements OnInit {
  shortIntro: string = '';
  fullIntro: string = '';
  isExpanded = false;
  memberInfo = input.required<Member>();

  ngOnInit(): void {
    this.shortIntro = this.memberInfo().introduction?.substring(0, 35);
    this.fullIntro = this.memberInfo().introduction;
  }

  toggleIntro() {
    this.isExpanded = !this.isExpanded;
  }
}
