import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GetMediaFilesResponse } from '../../../core/models/response-models/media-file-response.model';
import { MediaType } from '../../../core/models/enums/media-type';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-media-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './media-management.component.html',
  styleUrl: './media-management.component.css'
})
export class MediaManagementComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private formBuilder = inject(FormBuilder);

  public MediaType = MediaType;
  public media = signal<GetMediaFilesResponse[]>([]);
  public loading = signal(true);
  public isUploadMode = signal(false);
  public submitting = signal(false);
  public selectedFile = signal<File | null>(null);
  public activeFilter = signal<MediaType | 'all'>('all');

  public mediaForm!: FormGroup;

  ngOnInit(): void {
    this.initializeForm();
    this.loadMedia();
  }

  private initializeForm(): void {
    this.mediaForm = this.formBuilder.group({
      fileDescriptionUz: [''],
      fileDescriptionRu: [''],
      altTextUz: [''],
      altTextRu: [''],
      type: [MediaType.Image, [Validators.required]]
    });
  }

  private loadMedia(): void {
    this.apiService.getMediaFilesRequest({}).subscribe({
      next: (media) => {
        this.media.set(media);
        this.loading.set(false);
      },
      error: (error) => {
        this.notificationService.showError(error.message);
        this.loading.set(false);
      }
    });
  }

  public openUploadMode(): void {
    this.isUploadMode.set(true);
    this.mediaForm.reset({ type: MediaType.Image });
    this.selectedFile.set(null);
  }

  public cancelUpload(): void {
    this.isUploadMode.set(false);
    this.mediaForm.reset();
    this.selectedFile.set(null);
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      this.selectedFile.set(file);

      // Auto-detect media type
      if (file.type.startsWith('video/')) {
        this.mediaForm.patchValue({ type: MediaType.Video });
      } else if (file.type.startsWith('image/')) {
        this.mediaForm.patchValue({ type: MediaType.Image });
      }
    }
  }

  public uploadMedia(): void {
    if (this.mediaForm.invalid) {
      this.mediaForm.markAllAsTouched();
      return;
    }

    if (!this.selectedFile()) {
      this.notificationService.showWarning(
        this.translationService.currentLanguage() === 'uz' 
          ? 'Fayl tanlang' 
          : 'Выберите файл'
      );
      return;
    }

    this.submitting.set(true);
    const formValue = this.mediaForm.value;

    const request = {
      FileDescriptionUz: formValue.fileDescriptionUz || undefined,
      FileDescriptionRu: formValue.fileDescriptionRu || undefined,
      AltTextUz: formValue.altTextUz || undefined,
      AltTextRu: formValue.altTextRu || undefined,
      Type: formValue.type,
      File: this.selectedFile()!
    };

    this.apiService.uploadMediaFileRequest(request).subscribe({
      next: () => {
        this.notificationService.showSuccess(
          this.translationService.currentLanguage() === 'uz' 
            ? 'Fayl yuklandi' 
            : 'Файл загружен'
        );
        this.loadMedia();
        this.cancelUpload();
      },
      error: (error) => {
        this.notificationService.showError(error.message);
      },
      complete: () => {
        this.submitting.set(false);
      }
    });
  }

  public deleteMedia(fileId: string): void {
    if (!confirm(this.translationService.currentLanguage() === 'uz' 
      ? 'Faylni o\'chirmoqchimisiz?' 
      : 'Удалить файл?')) {
      return;
    }

    this.apiService.deleteMediaFileRequest({ FileId: fileId }).subscribe({
      next: () => {
        this.notificationService.showSuccess(
          this.translationService.currentLanguage() === 'uz' 
            ? 'Fayl o\'chirildi' 
            : 'Файл удален'
        );
        this.loadMedia();
      },
      error: (error) => {
        this.notificationService.showError(error.message);
      }
    });
  }

  public setFilter(filter: MediaType | 'all'): void {
    this.activeFilter.set(filter);
  }

  public filteredMedia(): GetMediaFilesResponse[] {
    if (this.activeFilter() === 'all') {
      return this.media();
    }
    return this.media().filter(m => m.Type === this.activeFilter()?.toString());
  }

  public getImageCount(): number {
    return this.media().filter(m => m.Type === MediaType.Image.toString()).length;
  }

  public getVideoCount(): number {
    return this.media().filter(m => m.Type === MediaType.Video.toString()).length;
  }

  public getMediaUrl(fileUrl: string): string {
    return `${environment.apiBaseUrl}/${fileUrl}`;
  }

  public getFileName(media: GetMediaFilesResponse): string {
    // Extract filename from URL
    const parts = media.FileUrl.split('/');
    return parts[parts.length - 1];
  }

  public getAltText(media: GetMediaFilesResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (media.AltTextUz || '') 
      : (media.AltTextRu || '');
  }

  public getFileDescription(media: GetMediaFilesResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (media.FileDescriptionUz || '') 
      : (media.FileDescriptionRu || '');
  }
}

