import {
  Component,
  HostListener,
  inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { AccountService } from '../../_services/account.service';
import { Member } from '../../_models/member';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditorComponent } from '../photo-editor/photo-editor.component';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [
    TabsModule,
    FormsModule,
    PhotoEditorComponent,
    TimeagoModule,
    DatePipe,
  ],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css',
})
export default class MemberEditComponent implements OnInit {
  private accountService = inject(AccountService);
  private membersService = inject(MembersService);
  private toastr = inject(ToastrService);

  @ViewChild('editForm') editForm?: NgForm;
  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
    if (this.editForm?.dirty) {
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
    this.membersService.updateMember(this.editForm?.value).subscribe({
      next: (_dummy) => {
        this.toastr.success('Profile Updated Successfully');
        this.editForm?.reset(this.memberModel);
      },
    });
  }

  onMemberChangeByPhotoEditor(event: Member) {
    this.memberModel = event;
  }
}
