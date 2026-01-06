import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { TranslationService } from '../../../core/services/translation.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';
import { NotificationComponent } from '../../../shared/components/notification/notification.component';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, NotificationComponent],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.css'
})
export class AdminLayoutComponent {
  public translationService = inject(TranslationService);
  private localStorageService = inject(LocalStorageService);
  private router = inject(Router);

  public sidebarOpen = signal(true);

  public toggleSidebar(): void {
    this.sidebarOpen.update(value => !value);
  }

  public logout(): void {
    this.localStorageService.removeAccessToken();
    this.router.navigate(['/admin/login']);
  }

  public changeLanguage(language: 'uz' | 'ru'): void {
    this.translationService.setLanguage(language);
  }
}

