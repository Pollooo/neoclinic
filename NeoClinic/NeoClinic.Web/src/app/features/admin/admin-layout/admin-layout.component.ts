import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { TranslationService } from '../../../core/services/translation.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';
import { ApiService } from '../../../core/services/api.service';
import { ThemeService } from '../../../core/services/theme.service';
import { GetContactMessageResponse } from '../../../core/models/response-models/contact-message-response.model';
import { NotificationComponent } from '../../../shared/components/notification/notification.component';
import { lightModeIcon } from '../../../shared/svg/light-mode';
import { nightModeIcon } from '../../../shared/svg/night-mode';


@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, NotificationComponent],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.css'
})
export class AdminLayoutComponent implements OnInit {
  public translationService = inject(TranslationService);
  private localStorageService = inject(LocalStorageService);
  private apiService = inject(ApiService);
  private router = inject(Router);
  public themeService = inject(ThemeService);
  private sanitizer = inject(DomSanitizer);

  public themeIcon = computed(() => {
    const svg = this.themeService.currentTheme() === 'light' ? nightModeIcon : lightModeIcon;
    return this.sanitizer.bypassSecurityTrustResourceUrl(
      'data:image/svg+xml,' + encodeURIComponent(svg)
    );
  });


  public sidebarOpen = signal(false);
  public contactInfo = signal<GetContactMessageResponse | null>(null);

  ngOnInit(): void {
    this.loadContactInfo();
    this.checkScreenSize();
    window.addEventListener('resize', () => this.checkScreenSize());
  }

  private checkScreenSize(): void {
    const isDesktop = window.innerWidth > 1024;
    this.sidebarOpen.set(isDesktop);
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

  public toggleSidebar(): void {
    this.sidebarOpen.update(value => !value);
  }

  public closeSidebarOnMobile(): void {
    if (window.innerWidth <= 1024) {
      this.sidebarOpen.set(false);
    }
  }

  public logout(): void {
    this.localStorageService.removeAccessToken();
    this.router.navigate(['/' + this.translationService.currentLanguage() + '/admin/login']);
  }

  public changeLanguage(language: 'uz' | 'ru'): void {
    const currentUrl = this.router.url;
    const newUrl = currentUrl.replace(/^\/(uz|ru)/, '/' + language);
    this.translationService.setLanguage(language);
    this.router.navigateByUrl(newUrl);
  }
}

