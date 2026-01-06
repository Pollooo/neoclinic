import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="notification-container">
      @for (notification of notificationService.notifications(); track notification.id) {
        <div class="notification notification-{{ notification.type }}" 
             (click)="notificationService.removeNotification(notification.id)">
          <div class="notification-icon">
            @switch (notification.type) {
              @case ('success') {
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              }
              @case ('error') {
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M9.75 9.75l4.5 4.5m0-4.5l-4.5 4.5M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              }
              @case ('warning') {
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126zM12 15.75h.007v.008H12v-.008z" />
                </svg>
              }
              @case ('info') {
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M11.25 11.25l.041-.02a.75.75 0 011.063.852l-.708 2.836a.75.75 0 001.063.853l.041-.021M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9-3.75h.008v.008H12V8.25z" />
                </svg>
              }
            }
          </div>
          <div class="notification-message">{{ notification.message }}</div>
          <button class="notification-close" (click)="notificationService.removeNotification(notification.id); $event.stopPropagation()">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    .notification-container {
      position: fixed;
      top: 20px;
      right: 20px;
      z-index: 9999;
      display: flex;
      flex-direction: column;
      gap: 12px;
      max-width: 400px;
    }

    .notification {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 16px;
      border-radius: 8px;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
      background: white;
      animation: slideIn 0.3s ease-out;
      cursor: pointer;
      transition: transform 0.2s;
    }

    .notification:hover {
      transform: translateX(-4px);
    }

    @keyframes slideIn {
      from {
        transform: translateX(400px);
        opacity: 0;
      }
      to {
        transform: translateX(0);
        opacity: 1;
      }
    }

    .notification-icon {
      flex-shrink: 0;
      width: 24px;
      height: 24px;
    }

    .notification-icon svg {
      width: 100%;
      height: 100%;
    }

    .notification-message {
      flex: 1;
      font-size: 14px;
      line-height: 1.5;
      color: #333;
    }

    .notification-close {
      flex-shrink: 0;
      width: 20px;
      height: 20px;
      border: none;
      background: none;
      cursor: pointer;
      padding: 0;
      opacity: 0.6;
      transition: opacity 0.2s;
    }

    .notification-close:hover {
      opacity: 1;
    }

    .notification-close svg {
      width: 100%;
      height: 100%;
    }

    .notification-success {
      border-left: 4px solid #10b981;
    }

    .notification-success .notification-icon {
      color: #10b981;
    }

    .notification-error {
      border-left: 4px solid #ef4444;
    }

    .notification-error .notification-icon {
      color: #ef4444;
    }

    .notification-warning {
      border-left: 4px solid #f59e0b;
    }

    .notification-warning .notification-icon {
      color: #f59e0b;
    }

    .notification-info {
      border-left: 4px solid #3b82f6;
    }

    .notification-info .notification-icon {
      color: #3b82f6;
    }

    @media (max-width: 640px) {
      .notification-container {
        left: 20px;
        right: 20px;
        max-width: none;
      }
    }
  `]
})
export class NotificationComponent {
  public notificationService = inject(NotificationService);
}

