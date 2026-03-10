import { Component, signal, OnInit, OnDestroy, inject, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';
import { Subscription } from 'rxjs';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../shared/components/footer/footer.component';
import { NotificationComponent } from '../../shared/components/notification/notification.component';
import { ContactInfoComponent } from '../../shared/components/contact-info/contact-info.component';
import { YandexMapComponent } from '../../shared/components/yandex-map/yandex-map.component';

@Component({
  selector: 'app-public-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NavbarComponent, FooterComponent, NotificationComponent, ContactInfoComponent, YandexMapComponent],
  templateUrl: './public-layout.component.html',
  styleUrl: './public-layout.component.css'
})
export class PublicLayoutComponent implements OnInit, OnDestroy {
  protected isNavigating = signal(false);
  protected showScrollTop = signal(false);

  private router = inject(Router);
  private routerSub!: Subscription;

  ngOnInit(): void {
    this.routerSub = this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        this.isNavigating.set(true);
      } else if (
        event instanceof NavigationEnd ||
        event instanceof NavigationCancel ||
        event instanceof NavigationError
      ) {
        this.isNavigating.set(false);
      }
    });
  }

  ngOnDestroy(): void {
    this.routerSub?.unsubscribe();
  }

  @HostListener('window:scroll')
  onScroll(): void {
    this.showScrollTop.set(window.scrollY > 320);
  }

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
