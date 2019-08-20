import {Component, Input, OnInit, EventEmitter, Output} from '@angular/core';
import {Photo} from 'src/app/_models/photo';
import {FileUploader} from 'ng2-file-upload';
import {environment} from 'src/environments/environment';
import {AuthService} from 'src/app/_services/auth.service';
import {AlertifyService} from 'src/app/_services/alertify.service';
import {UserService} from 'src/app/_services/user.service';

@Component({
  selector: 'app-photos-editor',
  templateUrl: './photos-editor.component.html',
  styleUrls: ['./photos-editor.component.css'],
})
export class PhotosEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  hasAnotherDropZoneOver = false;
  baseUrl = environment.apiUrl;
  maxFileSzie_10_Meg = 10 * 1024 * 1024; // 10 MB
  currentMain: Photo;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasAnotherDropZoneOver = e;
  }

  initializeUploader() {
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
      maxFileSize: this.maxFileSzie_10_Meg,
    });

    // Suppress a problem encountred during the upload process .
    this.uploader.onAfterAddingFile = file => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const result: Photo = JSON.parse(response);
        const photo = {
          id: result.id,
          url: result.url,
          dateAdded: result.dateAdded,
          description: result.description,
          isMain: result.isMain,
        };

        this.photos.push(photo);
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService
      .setMainPhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(
        () => {
          this.currentMain = this.photos.filter(p => p.isMain === true)[0];
          this.currentMain.isMain = false;
          photo.isMain = true;
          this.getMemberPhotoChange.emit(photo.url);
        },
        error => {
          this.alertify.error(error);
        }
      );
  }
}
