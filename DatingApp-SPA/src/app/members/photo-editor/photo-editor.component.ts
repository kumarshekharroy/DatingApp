import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/photo';

import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { error } from 'protractor';
import { AlerifyServices } from 'src/app/_services/alertify.service';
@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Output() ProfilePicChange = new EventEmitter<string>();
  @Input() photos: Photo[];
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentProfilePic: Photo;
  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlerifyServices
  ) {}

  ngOnInit() {
    this.initalizeUploader();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
  initalizeUploader() {
    this.uploader = new FileUploader({
      url:
        this.baseUrl +
        'users/' +
        this.authService.decodedToken.nameid +
        '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = file => (file.withCredentials = false);
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        // const photo:Photo={
        //   id:res.id,
        //   url:res.url,
        //   dateAdded:res.dateAdded
        // }
        this.photos.push(res);
      }
    };
  }

  setProfilePic(photo: Photo) {
    this.userService
      .setProfilePic(this.authService.decodedToken.nameid, photo.id)
      .subscribe(
        () => {
          this.currentProfilePic = this.photos.filter(
            p => p.isProfilePic === true
          )[0];
          this.currentProfilePic.isProfilePic = false;
          photo.isProfilePic = true;
          this.ProfilePicChange.emit(photo.url);
          this.authService.changeProfilePic(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem(
            'user',
            JSON.stringify(this.authService.currentUser)
          );
          // console.log('Successfully set as profile pic');
        },
        // tslint:disable-next-line: no-shadowed-variable
        error => {
          this.alertify.error(error);
        }
      );
  }
  deletePhoto(id: number) {
    this.alertify.confirm('Are you sure you want to delete this photo?', () => {
      this.userService
        .deletePhoto(this.authService.decodedToken.nameid, id)
        .subscribe(
          () => {
            this.photos.splice(
              this.photos.findIndex(p => p.id === id),
              1
            );
            this.alertify.success('Photo has been deleted');
          },
          () => {
            this.alertify.error('Failed to delete the photo');
          }
        );
    });
  }
}
