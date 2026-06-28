import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { GetMediaFilesResponse } from '../../../core/models/response-models/media-file-response.model';
import { MediaType } from '../../../core/models/enums/media-type';
import { environment } from '../../../environments/environment';
import { routes } from '../../../shared/routes';


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
  public showFilters = signal(false);

  ngOnInit(): void {
    this.loadMedia();
  }

  public toggleFilters(): void {
    this.showFilters.set(!this.showFilters());
  }

  public selectFilter(filter: MediaType | 'all'): void {
    this.activeFilter.set(filter);
    this.showFilters.set(false);
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

  public getImageCount(): number {
    return this.media().filter(m => m.type === MediaType.Image).length;
  }

  public getVideoCount(): number {
    return this.media().filter(m => m.type === MediaType.Video).length;
  }

  public filteredMedia(): GetMediaFilesResponse[] {
    if (this.activeFilter() === 'all') {
      return this.media();
    }
    return this.media().filter(m => m.type === this.activeFilter());
  }

  public getMediaUrl(fileUrl: string, blobName?: string): string {
    if (blobName) {
      return `${environment.apiBaseUrl}/${routes.media_files.proxy(blobName)}`;
    }
    return fileUrl;
  }

  public openLightbox(media: GetMediaFilesResponse): void {
    this.selectedMedia.set(media);

    if (media.type === MediaType.Video) {
      const link = document.createElement('link');
      link.rel = 'preload';
      link.as = 'video';
      link.href = this.getMediaUrl(media.fileUrl, media.blobName);
      link.id = 'video-preload';
      document.head.appendChild(link);
    }
  }

  public closeLightbox(): void {
    this.selectedMedia.set(null);
    const preloadLink = document.getElementById('video-preload');
    if (preloadLink) {
      preloadLink.remove();
    }
  }

  public getFileDescription(media: GetMediaFilesResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (media.fileDescriptionUz || '') 
      : (media.fileDescriptionRu || '');
  }

}
