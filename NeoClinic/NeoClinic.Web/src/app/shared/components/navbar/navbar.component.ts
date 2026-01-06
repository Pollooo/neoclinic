import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { TranslationService, Language } from '../../../core/services/translation.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  template: `
    <nav class="navbar">
      <div class="navbar-container">
        <div class="navbar-brand">
          <a routerLink="/" class="brand-link">
            <span class="brand-text">NeoClinic</span>
          </a>
        </div>

        <button class="mobile-menu-btn" (click)="toggleMobileMenu()">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
          </svg>
        </button>

        <div class="navbar-menu" [class.active]="mobileMenuOpen()">
          <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="nav-link">
            {{ translationService.t('common.home') }}
          </a>
          <a routerLink="/services" routerLinkActive="active" class="nav-link">
            {{ translationService.t('common.services') }}
          </a>
          <a routerLink="/doctors" routerLinkActive="active" class="nav-link">
            {{ translationService.t('common.doctors') }}
          </a>
          <a routerLink="/gallery" routerLinkActive="active" class="nav-link">
            {{ translationService.t('common.gallery') }}
          </a>
          <a routerLink="/contact" routerLinkActive="active" class="nav-link">
            {{ translationService.t('common.contact') }}
          </a>
          
          <a routerLink="/appointment/create" class="btn btn-primary">
            {{ translationService.t('common.appointment') }}
          </a>

          <div class="language-switcher">
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

    .brand-text {
      font-size: 1.5rem;
      font-weight: 700;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
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
      .mobile-menu-btn {
        display: block;
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

      .language-switcher {
        border-left: none;
        border-top: 1px solid #e5e7eb;
        padding-left: 0;
        padding-top: 1rem;
      }

      .nav-link.active::after {
        display: none;
      }
    }
  `]
})
export class NavbarComponent {
  public translationService = inject(TranslationService);
  public mobileMenuOpen = signal(false);

  public toggleMobileMenu(): void {
    this.mobileMenuOpen.update(value => !value);
  }

  public changeLanguage(language: Language): void {
    this.translationService.setLanguage(language);
    this.mobileMenuOpen.set(false);
  }
}

