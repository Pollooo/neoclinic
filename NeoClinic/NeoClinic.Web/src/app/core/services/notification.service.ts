import { Injectable, signal } from '@angular/core';

export interface Notification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
  duration?: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  public notifications = signal<Notification[]>([]);

  public showSuccess(message: string, duration: number = 5000): void {
    this.addNotification('success', message, duration);
  }

  public showError(message: string, duration: number = 7000): void {
    this.addNotification('error', message, duration);
  }

  public showWarning(message: string, duration: number = 5000): void {
    this.addNotification('warning', message, duration);
  }

  public showInfo(message: string, duration: number = 5000): void {
    this.addNotification('info', message, duration);
  }

  private addNotification(type: Notification['type'], message: string, duration: number): void {
    const id = Math.random().toString(36).substring(2, 9);
    const notification: Notification = { id, type, message, duration };
    
    this.notifications.update(notifications => [...notifications, notification]);

    if (duration > 0) {
      setTimeout(() => {
        this.removeNotification(id);
      }, duration);
    }
  }

  public removeNotification(id: string): void {
    this.notifications.update(notifications => 
      notifications.filter(notification => notification.id !== id)
    );
  }
}

