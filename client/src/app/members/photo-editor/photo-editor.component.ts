import { Component, inject, input, OnInit, output } from '@angular/core';
import { Member } from '../../_models/member';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { Photo } from '../../_models/photo';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [NgClass, NgIf, NgFor, FileUploadModule, DecimalPipe, NgStyle],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css',
})
export class PhotoEditorComponent implements OnInit {
  private accountService = inject(AccountService);
  private membersService = inject(MembersService);
  memberData = input.required<Member>();
  updatedMember = output<Member>();
  uploader?: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropzoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/upload-photo',
      authToken: 'Bearer ' + this.accountService.currentUser()?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (imgFile) => {
      imgFile.withCredentials = false;
    };

    /*
    
    <----------Very Imp--------->

    */

    // To changing the Payload of HTTP Request
    this.uploader.onBeforeUploadItem = (item) => {
      item.alias = 'imgFile'; // Set the form field name to match the backend parameter name
    };

    this.uploader.onSuccessItem = (item, resposne, status, headers) => {
      const photo = JSON.parse(resposne);

      const updatedMemberData = { ...this.memberData() };
      updatedMemberData.photos.push(photo);
      this.updatedMember.emit(updatedMemberData);

      if (photo.isMain) {
        const user = this.accountService.currentUser();

        if (user) {
          user.profilePhotoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }

        /* Parent -> updatedMember Photo also updated */
        const updatedMemberData = { ...this.memberData() };
        updatedMemberData.photoUrl = photo.url;

        // Making changes in the memberData from edit-profie comp
        updatedMemberData.photos.forEach((p) => {
          if (p.id == photo.id) p.isMain = true;
        });

        this.updatedMember.emit(updatedMemberData);
      }
    };
  }

  updateMemberPhoto(photo: Photo) {
    this.membersService.updateProfilePhoto(photo).subscribe({
      next: (_dummy) => {
        /* Account Service user Photo updated*/
        const user = this.accountService.currentUser();
        if (user) {
          user.profilePhotoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }

        /* Parent -> updatedMember Photo also updated */
        const updatedMemberData = { ...this.memberData() };
        updatedMemberData.photoUrl = photo.url;

        // Making changes in the memberData from edit-profie comp
        updatedMemberData.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          else if (p.id == photo.id) p.isMain = true;
        });

        this.updatedMember.emit(updatedMemberData);
      },
    });
  }

  deletePhoto(photo: Photo) {
    this.membersService.deletePhotoSvc(photo).subscribe({
      next: (_) => {
        const updatedMemberData = { ...this.memberData() };
        updatedMemberData.photos = updatedMemberData.photos.filter(
          (p) => p.id !== photo.id
        );
        this.updatedMember.emit(updatedMemberData);
      },
    });
  }
}
