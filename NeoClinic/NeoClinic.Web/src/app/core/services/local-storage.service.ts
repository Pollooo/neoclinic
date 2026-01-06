import { inject, Injectable } from '@angular/core';
import { DocumentService } from './document.service';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  private readonly documentService = inject(DocumentService);
  private readonly ACCESS_TOKEN_KEY = 'access_token';
  private readonly TOKEN_EXPIRY_KEY = 'token_expiry';
  private readonly LANGUAGE_KEY = 'language';

  public getAccessToken(): string | null {
    if (this.documentService.isBrowser()) {
      return localStorage.getItem(this.ACCESS_TOKEN_KEY);
    }
    return null;
  }

  public setAccessToken(token: string): void {
    if (this.documentService.isBrowser()) {
      localStorage.setItem(this.ACCESS_TOKEN_KEY, token);
      // Set token expiration to 1 hour from now
      const expiryTime = new Date().getTime() + (60 * 60 * 1000); // 1 hour in milliseconds
      localStorage.setItem(this.TOKEN_EXPIRY_KEY, expiryTime.toString());
    }
  }

  public removeAccessToken(): void {
    if (this.documentService.isBrowser()) {
      localStorage.removeItem(this.ACCESS_TOKEN_KEY);
      localStorage.removeItem(this.TOKEN_EXPIRY_KEY);
    }
  }

  public isAuthenticated(): boolean {
    if (!this.documentService.isBrowser()) {
      return false;
    }

    const token = this.getAccessToken();
    if (!token) {
      return false;
    }

    // Check if token has expired
    const expiryTime = localStorage.getItem(this.TOKEN_EXPIRY_KEY);
    if (expiryTime) {
      const now = new Date().getTime();
      const expiry = parseInt(expiryTime, 10);
      
      if (now > expiry) {
        // Token expired, remove it
        this.removeAccessToken();
        return false;
      }
    }

    return true;
  }

  public getLanguage(): string {
    if (this.documentService.isBrowser()) {
      return localStorage.getItem(this.LANGUAGE_KEY) || 'uz';
    }
    return 'uz';
  }

  public setLanguage(language: string): void {
    if (this.documentService.isBrowser()) {
      localStorage.setItem(this.LANGUAGE_KEY, language);
    }
  }
}

