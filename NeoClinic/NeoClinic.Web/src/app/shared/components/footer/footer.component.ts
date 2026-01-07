import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { GetContactMessageResponse } from '../../../core/models/response-models/contact-message-response.model';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <footer class="footer">
      <div class="footer-container">
        <div class="footer-grid">
          <div class="footer-section">
            <h3 class="footer-title">{{ contactInfo()?.name || 'Clinic' }}</h3>
            <p class="footer-description">
              {{ translationService.currentLanguage() === 'uz' 
                ? (contactInfo()?.aboutClinicUz || 'Zamonaviy tibbiy xizmatlar va professional shifokorlar')
                : (contactInfo()?.aboutClinicRu || 'Современные медицинские услуги и профессиональные врачи') }}
            </p>
            <div class="social-links">
              @if (contactInfo()?.facebookUrl) {
                <a [href]="contactInfo()?.facebookUrl" target="_blank" rel="noopener noreferrer" class="social-link" aria-label="Facebook">
                  <svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 24 24">
                    <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z"/>
                  </svg>
                </a>
              }
              @if (contactInfo()?.instagramUrl) {
                <a [href]="contactInfo()?.instagramUrl" target="_blank" rel="noopener noreferrer" class="social-link" aria-label="Instagram">
                  <svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 24 24">
                    <path d="M12 2.163c3.204 0 3.584.012 4.85.07 3.252.148 4.771 1.691 4.919 4.919.058 1.265.069 1.645.069 4.849 0 3.205-.012 3.584-.069 4.849-.149 3.225-1.664 4.771-4.919 4.919-1.266.058-1.644.07-4.85.07-3.204 0-3.584-.012-4.849-.07-3.26-.149-4.771-1.699-4.919-4.92-.058-1.265-.07-1.644-.07-4.849 0-3.204.013-3.583.07-4.849.149-3.227 1.664-4.771 4.919-4.919 1.266-.057 1.645-.069 4.849-.069zm0-2.163c-3.259 0-3.667.014-4.947.072-4.358.2-6.78 2.618-6.98 6.98-.059 1.281-.073 1.689-.073 4.948 0 3.259.014 3.668.072 4.948.2 4.358 2.618 6.78 6.98 6.98 1.281.058 1.689.072 4.948.072 3.259 0 3.668-.014 4.948-.072 4.354-.2 6.782-2.618 6.979-6.98.059-1.28.073-1.689.073-4.948 0-3.259-.014-3.667-.072-4.947-.196-4.354-2.617-6.78-6.979-6.98-1.281-.059-1.69-.073-4.949-.073zm0 5.838c-3.403 0-6.162 2.759-6.162 6.162s2.759 6.163 6.162 6.163 6.162-2.759 6.162-6.163c0-3.403-2.759-6.162-6.162-6.162zm0 10.162c-2.209 0-4-1.79-4-4 0-2.209 1.791-4 4-4s4 1.791 4 4c0 2.21-1.791 4-4 4zm6.406-11.845c-.796 0-1.441.645-1.441 1.44s.645 1.44 1.441 1.44c.795 0 1.439-.645 1.439-1.44s-.644-1.44-1.439-1.44z"/>
                  </svg>
                </a>
              }
              @if (contactInfo()?.telegramUrl) {
                <a [href]="contactInfo()?.telegramUrl" target="_blank" rel="noopener noreferrer" class="social-link" aria-label="Telegram">
                  <svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 24 24">
                    <path d="M11.944 0A12 12 0 0 0 0 12a12 12 0 0 0 12 12 12 12 0 0 0 12-12A12 12 0 0 0 12 0a12 12 0 0 0-.056 0zm4.962 7.224c.1-.002.321.023.465.14a.506.506 0 0 1 .171.325c.016.093.036.306.02.472-.18 1.898-.962 6.502-1.36 8.627-.168.9-.499 1.201-.82 1.23-.696.065-1.225-.46-1.9-.902-1.056-.693-1.653-1.124-2.678-1.8-1.185-.78-.417-1.21.258-1.91.177-.184 3.247-2.977 3.307-3.23.007-.032.014-.15-.056-.212s-.174-.041-.249-.024c-.106.024-1.793 1.14-5.061 3.345-.48.33-.913.49-1.302.48-.428-.008-1.252-.241-1.865-.44-.752-.245-1.349-.374-1.297-.789.027-.216.325-.437.893-.663 3.498-1.524 5.83-2.529 6.998-3.014 3.332-1.386 4.025-1.627 4.476-1.635z"/>
                  </svg>
                </a>
              }
            </div>
          </div>

          <div class="footer-section">
            <h4 class="footer-subtitle">{{ translationService.t('common.services') }}</h4>
            <ul class="footer-links">
              <li><a [routerLink]="['/' + translationService.currentLanguage() + '/services']">{{ translationService.t('common.services') }}</a></li>
              <li><a [routerLink]="['/' + translationService.currentLanguage() + '/doctors']">{{ translationService.t('common.doctors') }}</a></li>
              <li><a [routerLink]="['/' + translationService.currentLanguage() + '/gallery']">{{ translationService.t('common.gallery') }}</a></li>
            </ul>
          </div>

          <div class="footer-section">
            <h4 class="footer-subtitle">{{ translationService.t('common.contact') }}</h4>
            <ul class="footer-links">
              @if (contactInfo()?.phoneNumber) {
                <li>
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 6.75c0 8.284 6.716 15 15 15h2.25a2.25 2.25 0 002.25-2.25v-1.372c0-.516-.351-.966-.852-1.091l-4.423-1.106c-.44-.11-.902.055-1.173.417l-.97 1.293c-.282.376-.769.542-1.21.38a12.035 12.035 0 01-7.143-7.143c-.162-.441.004-.928.38-1.21l1.293-.97c.363-.271.527-.734.417-1.173L6.963 3.102a1.125 1.125 0 00-1.091-.852H4.5A2.25 2.25 0 002.25 4.5v2.25z" />
                  </svg>
                  <a [href]="'tel:' + contactInfo()?.phoneNumber">{{ contactInfo()?.phoneNumber }}</a>
                </li>
              }
              @if (contactInfo()?.email) {
                <li>
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75" />
                  </svg>
                  <a [href]="'mailto:' + contactInfo()?.email">{{ contactInfo()?.email }}</a>
                </li>
              }
              @if (contactInfo()?.locationUrl) {
                <li>
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M15 10.5a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1115 0z" />
                  </svg>
                  <a [href]="contactInfo()?.locationUrl" target="_blank" rel="noopener noreferrer">{{ translationService.currentLanguage() === 'uz' ? 'Manzil' : 'Адрес' }}</a>
                </li>
              }
            </ul>
          </div>

          <div class="footer-section">
            <h4 class="footer-subtitle">{{ translationService.t('contact.workingHours') }}</h4>
            <ul class="footer-links">
              <li>{{ translationService.currentLanguage() === 'uz' ? 'Dushanba - Juma' : 'Понедельник - Пятница' }}: 9:00 - 18:00</li>
              <li>{{ translationService.currentLanguage() === 'uz' ? 'Shanba' : 'Суббота' }}: 9:00 - 14:00</li>
              <li>{{ translationService.currentLanguage() === 'uz' ? 'Yakshanba: Yopiq' : 'Воскресенье: Выходной' }}</li>
            </ul>
          </div>
        </div>

        <div class="footer-bottom">
          <p>&copy; {{ currentYear }} {{ contactInfo()?.name || 'Clinic' }}. {{ translationService.currentLanguage() === 'uz' ? 'Barcha huquqlar himoyalangan' : 'Все права защищены' }}.</p>
        </div>
      </div>
    </footer>
  `,
  styles: [`
    .footer {
      background: #1f2937;
      color: #9ca3af;
      margin-top: auto;
    }

    .footer-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 3rem 2rem 1.5rem;
    }

    .footer-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 2rem;
      margin-bottom: 2rem;
    }

    .footer-section {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .footer-title {
      font-size: 1.5rem;
      font-weight: 700;
      color: white;
      margin: 0;
    }

    .footer-subtitle {
      font-size: 1.125rem;
      font-weight: 600;
      color: white;
      margin: 0;
    }

    .footer-description {
      font-size: 0.875rem;
      line-height: 1.6;
      margin: 0;
    }

    .social-links {
      display: flex;
      gap: 1rem;
    }

    .social-link {
      width: 40px;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: #374151;
      border-radius: 8px;
      color: #9ca3af;
      transition: all 0.3s;
    }

    .social-link:hover {
      background: #667eea;
      color: white;
      transform: translateY(-2px);
    }

    .social-link svg {
      width: 20px;
      height: 20px;
    }

    .footer-links {
      list-style: none;
      padding: 0;
      margin: 0;
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .footer-links li {
      font-size: 0.875rem;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .footer-links li svg {
      width: 16px;
      height: 16px;
      flex-shrink: 0;
    }

    .footer-links a {
      color: #9ca3af;
      text-decoration: none;
      transition: color 0.2s;
    }

    .footer-links a:hover {
      color: white;
    }

    .footer-bottom {
      padding-top: 2rem;
      border-top: 1px solid #374151;
      text-align: center;
      font-size: 0.875rem;
    }

    .footer-bottom p {
      margin: 0;
    }

    @media (max-width: 768px) {
      .footer-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class FooterComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  public currentYear = new Date().getFullYear();
  public contactInfo = signal<GetContactMessageResponse | null>(null);

  ngOnInit(): void {
    this.loadContactInfo();
  }

  private loadContactInfo(): void {
    this.apiService.getContactMessageRequest({}).subscribe({
      next: (response) => {
        console.log('Footer contact info received:', response);
        if (response) {
          this.contactInfo.set(response);
        }
      },
      error: (error) => {
        console.error('Failed to load footer contact info:', error);
      }
    });
  }
}

