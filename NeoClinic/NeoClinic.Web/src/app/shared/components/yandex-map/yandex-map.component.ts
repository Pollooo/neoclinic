import { Component, inject, OnInit, signal, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser, CommonModule } from '@angular/common';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { GetContactMessageResponse } from '../../../core/models/response-models/contact-message-response.model';

declare var ymaps: any;

@Component({
  selector: 'app-yandex-map',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (contactInfo()?.locationUrl) {
      <section class="map-section">
        <div class="map-container">
          <div class="map-header">
            <h2 class="map-title">
              {{ translationService.currentLanguage() === 'uz' ? 'Bizning manzil' : 'Наш адрес' }}
            </h2>
            <p class="map-subtitle">
              {{ translationService.currentLanguage() === 'uz' 
                ? 'Bizni topish oson' 
                : 'Нас легко найти' }}
            </p>
          </div>
          <div id="yandex-map" class="map"></div>
          <div class="map-footer">
            <a [href]="contactInfo()?.locationUrl" target="_blank" rel="noopener noreferrer" class="map-link">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 10.5a3 3 0 11-6 0 3 3 0 016 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1115 0z" />
              </svg>
              {{ translationService.currentLanguage() === 'uz' 
                ? 'Manzilga borish' 
                : 'Получить маршрут' }}
            </a>
          </div>
        </div>
      </section>
    }
  `,
  styles: [`
    .map-section {
      width: 100%;
      background: linear-gradient(to bottom, #f9fafb, #ffffff);
      padding: 4rem 0;
    }

    .map-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 2rem;
    }

    .map-header {
      text-align: center;
      margin-bottom: 2.5rem;
    }

    .map-title {
      font-size: 2.25rem;
      font-weight: 700;
      color: #1f2937;
      margin: 0 0 0.75rem 0;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .map-subtitle {
      font-size: 1.125rem;
      color: #6b7280;
      margin: 0;
    }

    .map {
      width: 100%;
      height: 450px;
      border-radius: 16px;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
      overflow: hidden;
      border: 2px solid #e5e7eb;
    }

    .map-footer {
      margin-top: 2rem;
      text-align: center;
    }

    .map-link {
      display: inline-flex;
      align-items: center;
      gap: 0.75rem;
      padding: 1rem 2rem;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      text-decoration: none;
      border-radius: 12px;
      font-size: 1.125rem;
      font-weight: 600;
      transition: all 0.3s ease;
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
    }

    .map-link:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 24px rgba(102, 126, 234, 0.5);
    }

    .map-link svg {
      width: 24px;
      height: 24px;
    }

    @media (max-width: 768px) {
      .map-section {
        padding: 3rem 0;
      }

      .map-container {
        padding: 0 1rem;
      }

      .map-title {
        font-size: 1.75rem;
      }

      .map-subtitle {
        font-size: 1rem;
      }

      .map {
        height: 350px;
        border-radius: 12px;
      }

      .map-link {
        padding: 0.875rem 1.5rem;
        font-size: 1rem;
      }
    }

    @media (max-width: 480px) {
      .map {
        height: 300px;
      }
    }
  `]
})
export class YandexMapComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  private platformId = inject(PLATFORM_ID);
  
  public contactInfo = signal<GetContactMessageResponse | null>(null);
  private mapInitialized = false;

  ngOnInit(): void {
    this.loadContactInfo();
    if (isPlatformBrowser(this.platformId)) {
      this.loadYandexMapsScript();
    }
  }

  private loadContactInfo(): void {
    this.apiService.getContactMessageRequest({}).subscribe({
      next: (response) => {
        console.log('Map contact info received:', response);
        if (response) {
          this.contactInfo.set(response);
          // Try to initialize map after contact info is loaded
          if (isPlatformBrowser(this.platformId) && !this.mapInitialized) {
            this.initMap();
          }
        }
      },
      error: (error) => {
        console.error('Failed to load map contact info:', error);
      }
    });
  }

  private loadYandexMapsScript(): void {
    if (typeof ymaps !== 'undefined') {
      this.initMap();
      return;
    }

    const script = document.createElement('script');
    script.src = 'https://api-maps.yandex.ru/2.1/?apikey=&lang=ru_RU';
    script.type = 'text/javascript';
    script.onload = () => {
      this.initMap();
    };
    document.head.appendChild(script);
  }

  private initMap(): void {
    if (this.mapInitialized || typeof ymaps === 'undefined') {
      return;
    }

    ymaps.ready(() => {
      const mapElement = document.getElementById('yandex-map');
      if (!mapElement) {
        return;
      }

      // Default coordinates (Tashkent center as fallback)
      let coordinates = [41.2995, 69.2401];
      let zoomLevel = 15;

      // Try to extract coordinates from locationUrl
      const locationUrl = this.contactInfo()?.locationUrl;
      if (locationUrl) {
        coordinates = this.extractCoordinatesFromUrl(locationUrl);
      }

      const map = new ymaps.Map('yandex-map', {
        center: coordinates,
        zoom: zoomLevel,
        controls: ['zoomControl', 'fullscreenControl', 'geolocationControl']
      });

      // Add a placemark
      const placemark = new ymaps.Placemark(coordinates, {
        balloonContent: this.contactInfo()?.name || 'Clinic Location',
        hintContent: this.translationService.currentLanguage() === 'uz' 
          ? 'Klinika manzili' 
          : 'Адрес клиники'
      }, {
        preset: 'islands#medicalIcon',
        iconColor: '#667eea'
      });

      map.geoObjects.add(placemark);
      this.mapInitialized = true;
    });
  }

  private extractCoordinatesFromUrl(url: string): [number, number] {
    try {
      // Try to extract coordinates from Yandex Maps URL
      // Format: https://yandex.uz/maps/?ll=69.240562%2C41.311151&z=12
      const llMatch = url.match(/ll=([\d.]+)%2C([\d.]+)/);
      if (llMatch) {
        return [parseFloat(llMatch[2]), parseFloat(llMatch[1])];
      }

      // Format: https://yandex.uz/maps/?pt=69.240562,41.311151&z=12
      const ptMatch = url.match(/pt=([\d.]+),([\d.]+)/);
      if (ptMatch) {
        return [parseFloat(ptMatch[2]), parseFloat(ptMatch[1])];
      }

      // Format with @ in URL: @lat,lon,zoom
      const atMatch = url.match(/@([\d.]+),([\d.]+)/);
      if (atMatch) {
        return [parseFloat(atMatch[1]), parseFloat(atMatch[2])];
      }

      // Try Google Maps format: @lat,lon
      const googleMatch = url.match(/[@/]([-\d.]+),([-\d.]+)/);
      if (googleMatch) {
        return [parseFloat(googleMatch[1]), parseFloat(googleMatch[2])];
      }
    } catch (error) {
      console.error('Error extracting coordinates:', error);
    }

    // Return default Tashkent coordinates if extraction fails
    return [41.2995, 69.2401];
  }
}

