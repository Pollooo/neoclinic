import { Injectable, inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class FaviconService {
  private document = inject(DOCUMENT);

  setFavicon(iconUrl: string): void {
    const link: HTMLLinkElement = this.document.querySelector("link[rel*='icon']") || this.document.createElement('link');
    link.type = 'image/x-icon';
    link.rel = 'icon';
    link.href = iconUrl;
    
    if (!this.document.querySelector("link[rel*='icon']")) {
      this.document.getElementsByTagName('head')[0].appendChild(link);
    }
  }

  setTitle(title: string): void {
    this.document.title = title;
  }
}

