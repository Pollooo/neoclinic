import { Component, signal, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ApiService } from './core/services/api.service';
import { FaviconService } from './core/services/favicon.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('Clinic');
  private apiService = inject(ApiService);
  private faviconService = inject(FaviconService);

  ngOnInit(): void {
    this.loadBranding();
  }

  private loadBranding(): void {
    // Load contact info for clinic name
    this.apiService.getContactMessageRequest({}).subscribe({
      next: (info) => {
        if (info?.name) {
          this.faviconService.setTitle(info.name);
        }
      },
      error: (error) => {
        console.error('Failed to load clinic name:', error);
      }
    });

    // Load logo for favicon
    this.apiService.getMediaFilesRequest({}).subscribe({
      next: (media) => {
        const logoMedia = media.find(m => 
          m.fileDescriptionUz?.toLowerCase().includes('clinic name') || 
          m.fileDescriptionRu?.toLowerCase().includes('clinic name')
        );
        if (logoMedia) {
          this.faviconService.setFavicon(logoMedia.fileUrl);
        }
      },
      error: (error) => {
        console.error('Failed to load favicon:', error);
      }
    });
  }
}
