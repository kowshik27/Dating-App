import { Component, HostListener, inject, OnInit, ViewChild } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { AccountService } from '../../_services/account.service';
import { Member } from '../../_models/member';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [TabsModule, FormsModule],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css',
})
export default class MemberEditComponent implements OnInit {
  private accountService = inject(AccountService);
  private membersService = inject(MembersService);
  private toastr = inject(ToastrService);

  @ViewChild('editForm') editForm?: NgForm;
  @HostListener('window:beforeunload', ['$event']) notify($event:any){
      if(this.editForm?.dirty){
        $event.returnValue = true;
      }
  }

  memberModel?: Member;

  ngOnInit(): void {
    this.loadMemberData();
  }

  loadMemberData() {
    const user = this.accountService.currentUser();
    if (!user) return;

    this.membersService.getMember(user.username).subscribe({
      next: (memberRes) => {
        this.memberModel = memberRes;
      },
    });
  }

  updateMemberData() {
    console.log(this.memberModel);
    this.toastr.success('Profile Updated Successfully');
    this.editForm?.reset(this.memberModel);
  }
}
