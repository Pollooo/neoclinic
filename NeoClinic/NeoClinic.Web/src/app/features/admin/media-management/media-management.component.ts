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
  public previewUrl = signal<string | null>(null);
  public isDragging = signal(false);
  public selectedMediaForView = signal<GetMediaFilesResponse | null>(null);

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
    this.previewUrl.set(null);
  }

  public cancelUpload(): void {
    this.isUploadMode.set(false);
    this.mediaForm.reset();
    this.selectedFile.set(null);
    this.previewUrl.set(null);
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.processFile(input.files[0]);
    }
  }

  public onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(true);
  }

  public onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(false);
  }

  public onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(false);

    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      const file = files[0];
      if (file.type.startsWith('image/') || file.type.startsWith('video/')) {
        this.processFile(file);
      } else {
        this.notificationService.showError(
          this.translationService.currentLanguage() === 'uz' 
            ? 'Faqat rasm yoki video fayllarini yuklash mumkin' 
            : 'Можно загружать только изображения или видео'
        );
      }
    }
  }

  private processFile(file: File): void {
    // Validate file type
    if (!file.type.startsWith('image/') && !file.type.startsWith('video/')) {
      this.notificationService.showError(
        this.translationService.currentLanguage() === 'uz' 
          ? 'Faqat rasm yoki video fayllarini yuklash mumkin' 
          : 'Можно загружать только изображения или видео'
      );
      return;
    }

    // Validate file size (max 50MB for videos, 10MB for images)
    const maxSize = file.type.startsWith('video/') ? 50 * 1024 * 1024 : 10 * 1024 * 1024;
    if (file.size > maxSize) {
      const maxSizeMB = file.type.startsWith('video/') ? '50MB' : '10MB';
      this.notificationService.showError(
        this.translationService.currentLanguage() === 'uz' 
          ? `Fayl hajmi ${maxSizeMB} dan oshmasligi kerak` 
          : `Размер файла не должен превышать ${maxSizeMB}`
      );
      return;
    }

    this.selectedFile.set(file);

    // Auto-detect media type
    if (file.type.startsWith('video/')) {
      this.mediaForm.patchValue({ type: MediaType.Video });
    } else if (file.type.startsWith('image/')) {
      this.mediaForm.patchValue({ type: MediaType.Image });
    }

    // Create preview
    const reader = new FileReader();
    reader.onload = (e: ProgressEvent<FileReader>) => {
      this.previewUrl.set(e.target?.result as string);
    };
    reader.readAsDataURL(file);
  }

  public removeSelectedFile(): void {
    this.selectedFile.set(null);
    this.previewUrl.set(null);
  }

  public isImage(): boolean {
    return this.selectedFile()?.type.startsWith('image/') || false;
  }

  public isVideo(): boolean {
    return this.selectedFile()?.type.startsWith('video/') || false;
  }

  public onVideoError(event: Event, mediaItem?: GetMediaFilesResponse): void {
    const videoElement = event.target as HTMLVideoElement;
    console.error('Video loading error:', {
      error: videoElement.error,
      src: videoElement.currentSrc,
      readyState: videoElement.readyState,
      networkState: videoElement.networkState,
      mediaItem: mediaItem
    });
  }

  public onVideoLoaded(event: Event): void {
    const videoElement = event.target as HTMLVideoElement;
    videoElement.setAttribute('data-loaded', 'true');
    console.log('Video loaded successfully:', event);
  }

  public openMediaLightbox(media: GetMediaFilesResponse): void {
    this.selectedMediaForView.set(media);
  }

  public closeMediaLightbox(): void {
    this.selectedMediaForView.set(null);
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
      fileDescriptionUz: formValue.fileDescriptionUz || undefined,
      fileDescriptionRu: formValue.fileDescriptionRu || undefined,
      altTextUz: formValue.altTextUz || undefined,
      altTextRu: formValue.altTextRu || undefined,
      type: formValue.type,
      file: this.selectedFile()!
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
    return this.media().filter(m => m.type === this.activeFilter());
  }

  public getImageCount(): number {
    return this.media().filter(m => m.type === MediaType.Image).length;
  }

  public getVideoCount(): number {
    return this.media().filter(m => m.type === MediaType.Video).length;
  }

  public getMediaUrl(fileUrl: string): string {
    // fileUrl is already a complete Azure Blob Storage URL
    return fileUrl;
  }

  public getFileName(media: GetMediaFilesResponse): string {
    // Extract filename from URL
    const parts = media.fileUrl.split('/');
    return parts[parts.length - 1];
  }

  public getAltText(media: GetMediaFilesResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (media.altTextUz || '') 
      : (media.altTextRu || '');
  }

  public getFileDescription(media: GetMediaFilesResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (media.fileDescriptionUz || '') 
      : (media.fileDescriptionRu || '');
  }
}

