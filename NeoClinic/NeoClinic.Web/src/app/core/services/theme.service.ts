import { Injectable, signal, effect } from '@angular/core';

export type Theme = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  public currentTheme = signal<Theme>('light');

  constructor() {
    const saved = localStorage.getItem('admin-theme') as Theme | null;
    if (saved === 'light' || saved === 'dark') {
      this.currentTheme.set(saved);
    }
    this.applyTheme(this.currentTheme());
  }

  public toggle(): void {
    const next = this.currentTheme() === 'light' ? 'dark' : 'light';
    this.currentTheme.set(next);
    localStorage.setItem('admin-theme', next);
    this.applyTheme(next);
  }

  private applyTheme(theme: Theme): void {
    document.documentElement.setAttribute('data-theme', theme);
  }
}