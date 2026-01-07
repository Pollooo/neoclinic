import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { TranslationService, Language } from '../../../core/services/translation.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';
import { ApiService } from '../../../core/services/api.service';
import { GetContactMessageResponse } from '../../../core/models/response-models/contact-message-response.model';
import { GetMediaFilesResponse } from '../../../core/models/response-models/media-file-response.model';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  template: `
    <nav class="navbar">
      <div class="navbar-container">
        <div class="navbar-brand">
          <a [routerLink]="['/' + translationService.currentLanguage()]" class="brand-link">
            @if (logoUrl()) {
              <img [src]="logoUrl()" alt="Logo" class="brand-logo" />
            } @else {
              <span class="brand-text">{{ contactInfo()?.name || 'Clinic' }}</span>
            }
          </a>
        </div>

        <div class="mobile-controls">
          <div class="language-switcher language-switcher-mobile">
            <button 
              class="lang-btn" 
              [class.active]="translationService.currentLanguage() === 'uz'"
              (click)="changeLanguage('uz')">
              UZ
            </button>
            <button 
              class="lang-btn" 
              [class.active]="translationService.currentLanguage() === 'ru'"
              (click)="changeLanguage('ru')">
              RU
            </button>
          </div>

          <button class="mobile-menu-btn" (click)="toggleMobileMenu()">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
            </svg>
          </button>
        </div>

        <div class="navbar-menu" [class.active]="mobileMenuOpen()">
          <a [routerLink]="['/' + translationService.currentLanguage()]" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="nav-link">
            {{ translationService.t('common.home') }}
          </a>
          <a [routerLink]="['/' + translationService.currentLanguage() + '/services']" routerLinkActive="active" class="nav-link">
            {{ translationService.t('common.services') }}
          </a>
          <a [routerLink]="['/' + translationService.currentLanguage() + '/doctors']" routerLinkActive="active" class="nav-link">
            {{ translationService.t('common.doctors') }}
          </a>
          <a [routerLink]="['/' + translationService.currentLanguage() + '/gallery']" routerLinkActive="active" class="nav-link">
            {{ translationService.t('common.gallery') }}
          </a>
          
          <a [routerLink]="['/' + translationService.currentLanguage() + '/appointment/create']" class="btn btn-primary">
            {{ translationService.t('common.appointment') }}
          </a>

          @if (contactInfo()) {
            <div class="contact-info-header">
              @if (contactInfo()?.phoneNumber) {
                <a [href]="'tel:' + contactInfo()?.phoneNumber" class="contact-link">
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M2.25 6.75c0 8.284 6.716 15 15 15h2.25a2.25 2.25 0 002.25-2.25v-1.372c0-.516-.351-.966-.852-1.091l-4.423-1.106c-.44-.11-.902.055-1.173.417l-.97 1.293c-.282.376-.769.542-1.21.38a12.035 12.035 0 01-7.143-7.143c-.162-.441.004-.928.38-1.21l1.293-.97c.363-.271.527-.734.417-1.173L6.963 3.102a1.125 1.125 0 00-1.091-.852H4.5A2.25 2.25 0 002.25 4.5v2.25z" />
                  </svg>
                  <span>{{ contactInfo()?.phoneNumber }}</span>
                </a>
              }
              @if (contactInfo()?.email) {
                <a [href]="'mailto:' + contactInfo()?.email" class="contact-link">
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75" />
                  </svg>
                  <span>{{ contactInfo()?.email }}</span>
                </a>
              }
            </div>
          }

          <div class="language-switcher language-switcher-desktop">
            <button 
              class="lang-btn" 
              [class.active]="translationService.currentLanguage() === 'uz'"
              (click)="changeLanguage('uz')">
              UZ
            </button>
            <button 
              class="lang-btn" 
              [class.active]="translationService.currentLanguage() === 'ru'"
              (click)="changeLanguage('ru')">
              RU
            </button>
          </div>
        </div>
      </div>
    </nav>
  `,
  styles: [`
    .navbar {
      background: white;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      position: sticky;
      top: 0;
      z-index: 1000;
    }

    .navbar-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 1rem 2rem;
      display: flex;
      align-items: center;
      justify-content: space-between;
    }

    .navbar-brand .brand-link {
      text-decoration: none;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .brand-logo {
      height: 50px;
      width: auto;
      object-fit: contain;
    }

    .brand-text {
      font-size: 1.5rem;
      font-weight: 700;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .mobile-controls {
      display: none;
      align-items: center;
      gap: 1rem;
    }

    .mobile-menu-btn {
      display: none;
      background: none;
      border: none;
      cursor: pointer;
      padding: 0.5rem;
      color: #333;
    }

    .mobile-menu-btn svg {
      width: 24px;
      height: 24px;
    }

    .language-switcher-mobile {
      display: none;
    }

    .language-switcher-desktop {
      display: flex;
    }

    .navbar-menu {
      display: flex;
      align-items: center;
      gap: 2rem;
    }

    .nav-link {
      text-decoration: none;
      color: #4b5563;
      font-weight: 500;
      transition: color 0.2s;
      position: relative;
    }

    .nav-link:hover,
    .nav-link.active {
      color: #667eea;
    }

    .nav-link.active::after {
      content: '';
      position: absolute;
      bottom: -8px;
      left: 0;
      right: 0;
      height: 2px;
      background: #667eea;
    }

    .btn {
      padding: 0.5rem 1.5rem;
      border-radius: 8px;
      text-decoration: none;
      font-weight: 600;
      transition: all 0.2s;
      border: none;
      cursor: pointer;
    }

    .btn-primary {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
    }

    .contact-info-header {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
      padding-left: 1rem;
      border-left: 1px solid #e5e7eb;
    }

    .contact-link {
      display: flex;
      align-items: center;
      gap: 0.375rem;
      text-decoration: none;
      color: #6b7280;
      font-size: 0.875rem;
      font-weight: 500;
      transition: color 0.2s;
    }

    .contact-link:hover {
      color: #667eea;
    }

    .contact-link svg {
      width: 16px;
      height: 16px;
    }

    .language-switcher {
      display: flex;
      gap: 0.5rem;
      padding-left: 1rem;
      border-left: 1px solid #e5e7eb;
    }

    .lang-btn {
      padding: 0.25rem 0.75rem;
      border: 2px solid #e5e7eb;
      background: white;
      border-radius: 6px;
      font-weight: 600;
      color: #6b7280;
      cursor: pointer;
      transition: all 0.2s;
    }

    .lang-btn:hover {
      border-color: #667eea;
      color: #667eea;
    }

    .lang-btn.active {
      background: #667eea;
      border-color: #667eea;
      color: white;
    }

    @media (max-width: 768px) {
      .mobile-controls {
        display: flex;
      }

      .mobile-menu-btn {
        display: block;
      }

      .language-switcher-mobile {
        display: flex;
        border-left: none;
        padding-left: 0;
      }

      .language-switcher-desktop {
        display: none;
      }

      .navbar-menu {
        position: absolute;
        top: 100%;
        left: 0;
        right: 0;
        background: white;
        flex-direction: column;
        padding: 1rem;
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        display: none;
        gap: 1rem;
      }

      .navbar-menu.active {
        display: flex;
      }

      .contact-info-header {
        flex-direction: column;
        border-left: none;
        border-top: 1px solid #e5e7eb;
        padding-left: 0;
        padding-top: 1rem;
        gap: 0.5rem;
      }

      .nav-link.active::after {
        display: none;
      }
    }
  `]
})
export class NavbarComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  public mobileMenuOpen = signal(false);
  public contactInfo = signal<GetContactMessageResponse | null>(null);
  public logoUrl = signal<string | null>(null);

  ngOnInit(): void {
    this.loadContactInfo();
    this.loadLogo();
  }

  private loadContactInfo(): void {
    this.apiService.getContactMessageRequest({}).subscribe({
      next: (info) => {
        this.contactInfo.set(info);
      },
      error: (error) => {
        console.error('Failed to load contact info:', error);
      }
    });
  }

  private loadLogo(): void {
    this.apiService.getMediaFilesRequest({}).subscribe({
      next: (media) => {
        const logoMedia = media.find(m => 
          m.fileDescriptionUz?.toLowerCase().includes('clinic name') || 
          m.fileDescriptionRu?.toLowerCase().includes('clinic name')
        );
        if (logoMedia) {
          this.logoUrl.set(logoMedia.fileUrl);
        }
      },
      error: (error) => {
        console.error('Failed to load logo:', error);
      }
    });
  }

  public toggleMobileMenu(): void {
    this.mobileMenuOpen.update(value => !value);
  }

  public changeLanguage(language: Language): void {
    this.translationService.setLanguage(language);
    this.mobileMenuOpen.set(false);
  }
}

