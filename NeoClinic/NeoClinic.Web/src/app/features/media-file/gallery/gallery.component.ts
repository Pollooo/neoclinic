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
        this.media.set(media);
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
    return this.media().filter(m => m.Type === this.activeFilter()?.toString());
  }

  public getMediaUrl(fileUrl: string): string {
    return `${environment.apiBaseUrl}/${fileUrl}`;
  }

  public openLightbox(media: GetMediaFilesResponse): void {
    this.selectedMedia.set(media);
  }

  public closeLightbox(): void {
    this.selectedMedia.set(null);
  }
}
