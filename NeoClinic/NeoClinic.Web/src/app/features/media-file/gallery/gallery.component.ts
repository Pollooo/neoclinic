import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { GetMediaFilesResponse } from '../../../core/models/response-models/media-file-response.model';
import { MediaType } from '../../../core/models/enums/media-type';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-gallery',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './gallery.component.html',
  styleUrl: './gallery.component.css'
})
export class GalleryComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);

  public MediaType = MediaType;
  public media = signal<GetMediaFilesResponse[]>([]);
  public loading = signal(true);
  public activeFilter = signal<MediaType | 'all'>('all');
  public selectedMedia = signal<GetMediaFilesResponse | null>(null);

  ngOnInit(): void {
    this.loadMedia();
  }

  private loadMedia(): void {
    this.apiService.getMediaFilesRequest({}).subscribe({
      next: (media) => {
        // Filter out background, clinic name, and logo images
        const filteredMedia = media.filter(m => {
          const descUz = m.fileDescriptionUz?.toLowerCase() || '';
          const descRu = m.fileDescriptionRu?.toLowerCase() || '';
          return !descUz.includes('background') && 
                 !descRu.includes('background') && 
                 !descUz.includes('clinic name') && 
                 !descRu.includes('clinic name') &&
                 !descUz.includes('logo') && 
                 !descRu.includes('logo');
        });
        this.media.set(filteredMedia);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Failed to load media:', error);
        this.loading.set(false);
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

  public getMediaUrl(fileUrl: string): string {
    // fileUrl is already a complete Azure Blob Storage URL
    return fileUrl;
  }

  public openLightbox(media: GetMediaFilesResponse): void {
    this.selectedMedia.set(media);
  }

  public closeLightbox(): void {
    this.selectedMedia.set(null);
  }

  public onVideoError(event: Event, media?: GetMediaFilesResponse): void {
    const videoElement = event.target as HTMLVideoElement;
    console.error('Video loading error:', {
      error: videoElement.error,
      src: videoElement.currentSrc,
      readyState: videoElement.readyState,
      networkState: videoElement.networkState,
      media: media
    });
  }

  public onVideoLoaded(event: Event): void {
    const videoElement = event.target as HTMLVideoElement;
    videoElement.setAttribute('data-loaded', 'true');
    console.log('Video loaded successfully:', event);
  }

  public getFileDescription(media: GetMediaFilesResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (media.fileDescriptionUz || '') 
      : (media.fileDescriptionRu || '');
  }
}
