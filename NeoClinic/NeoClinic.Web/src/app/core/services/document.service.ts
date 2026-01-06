import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private platformId = inject(PLATFORM_ID);

  public isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }
}

