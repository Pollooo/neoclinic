import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="notif-container" role="region" aria-live="polite" aria-label="Notifications">
      @for (n of notificationService.notifications(); track n.id) {
        <div
          class="notif notif-{{ n.type }}"
          [style.--dur]="(n.duration ?? 5000) + 'ms'"
          (click)="notificationService.removeNotification(n.id)"
          role="alert"
        >
          <!-- Coloured accent strip -->
          <div class="notif-accent"></div>

          <!-- Icon -->
          <div class="notif-icon">
            @switch (n.type) {
              @case ('success') {
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
              }
              @case ('error') {
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M9.75 9.75l4.5 4.5m0-4.5l-4.5 4.5M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
              }
              @case ('warning') {
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126zM12 15.75h.007v.008H12v-.008z"/>
                </svg>
              }
              @case ('info') {
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M11.25 11.25l.041-.02a.75.75 0 011.063.852l-.708 2.836a.75.75 0 001.063.853l.041-.021M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9-3.75h.008v.008H12V8.25z"/>
                </svg>
              }
            }
          </div>

          <!-- Message -->
          <div class="notif-body">
            <span class="notif-label">
              @switch (n.type) {
                @case ('success') { Success }
                @case ('error')   { Error }
                @case ('warning') { Warning }
                @case ('info')    { Info }
              }
            </span>
            <p class="notif-msg">{{ n.message }}</p>
          </div>

          <!-- Close -->
          <button
            class="notif-close"
            (click)="notificationService.removeNotification(n.id); $event.stopPropagation()"
            aria-label="Dismiss"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12"/>
            </svg>
          </button>

          <!-- Auto-dismiss progress bar -->
          <div class="notif-progress"></div>
        </div>
      }
    </div>
  `,
  styles: [`
    .notif-container {
      position: fixed;
      top: 20px;
      right: 20px;
      z-index: 10000;
      display: flex;
      flex-direction: column;
      gap: 10px;
      width: 360px;
      max-width: calc(100vw - 40px);
    }

    .notif {
      position: relative;
      display: flex;
      align-items: flex-start;
      gap: 12px;
      padding: 14px 14px 18px 14px;
      border-radius: 12px;
      background: rgba(255, 255, 255, 0.97);
      backdrop-filter: blur(12px);
      -webkit-backdrop-filter: blur(12px);
      box-shadow:
        0 4px 24px rgba(0, 0, 0, 0.10),
        0 1px 6px rgba(0, 0, 0, 0.06),
        inset 0 0 0 1px rgba(255, 255, 255, 0.8);
      animation: notifIn 0.32s cubic-bezier(0.34, 1.56, 0.64, 1) both;
      cursor: pointer;
      overflow: hidden;
      transition: transform 0.18s ease, box-shadow 0.18s ease;
    }

    .notif:hover {
      transform: translateX(-3px) translateY(-1px);
      box-shadow:
        0 8px 32px rgba(0, 0, 0, 0.13),
        0 2px 8px rgba(0, 0, 0, 0.07);
    }

    @keyframes notifIn {
      from {
        transform: translateX(110%) scale(0.9);
        opacity: 0;
      }
      to {
        transform: translateX(0) scale(1);
        opacity: 1;
      }
    }

    /* Coloured left accent strip */
    .notif-accent {
      position: absolute;
      top: 0;
      left: 0;
      bottom: 0;
      width: 4px;
      border-radius: 12px 0 0 12px;
    }

    /* Icon */
    .notif-icon {
      flex-shrink: 0;
      width: 22px;
      height: 22px;
      margin-top: 1px;
    }

    .notif-icon svg {
      width: 100%;
      height: 100%;
    }

    /* Body */
    .notif-body {
      flex: 1;
      min-width: 0;
    }

    .notif-label {
      display: block;
      font-size: 11px;
      font-weight: 700;
      letter-spacing: 0.08em;
      text-transform: uppercase;
      margin-bottom: 2px;
      opacity: 0.65;
    }

    .notif-msg {
      margin: 0;
      font-size: 13.5px;
      line-height: 1.5;
      color: #1f2937;
      font-weight: 500;
    }

    /* Close button */
    .notif-close {
      flex-shrink: 0;
      width: 22px;
      height: 22px;
      border: none;
      background: rgba(0, 0, 0, 0.05);
      border-radius: 6px;
      cursor: pointer;
      padding: 3px;
      color: #6b7280;
      transition: background 0.15s, color 0.15s;
      margin-top: 1px;
    }

    .notif-close:hover {
      background: rgba(0, 0, 0, 0.12);
      color: #111;
    }

    .notif-close svg {
      width: 100%;
      height: 100%;
    }

    /* Auto-dismiss progress bar */
    .notif-progress {
      position: absolute;
      bottom: 0;
      left: 0;
      height: 3px;
      border-radius: 0 0 12px 12px;
      animation: notifTimer var(--dur, 5000ms) linear forwards;
    }

    @keyframes notifTimer {
      from { width: 100%; }
      to   { width: 0%;   }
    }

    /* ── Colour tokens per type ── */
    .notif-success .notif-accent,
    .notif-success .notif-progress { background: #10b981; }
    .notif-success .notif-icon     { color: #10b981; }
    .notif-success .notif-label    { color: #10b981; }

    .notif-error .notif-accent,
    .notif-error .notif-progress   { background: #ef4444; }
    .notif-error .notif-icon       { color: #ef4444; }
    .notif-error .notif-label      { color: #ef4444; }

    .notif-warning .notif-accent,
    .notif-warning .notif-progress { background: #f59e0b; }
    .notif-warning .notif-icon     { color: #f59e0b; }
    .notif-warning .notif-label    { color: #f59e0b; }

    .notif-info .notif-accent,
    .notif-info .notif-progress    { background: #667eea; }
    .notif-info .notif-icon        { color: #667eea; }
    .notif-info .notif-label       { color: #667eea; }

    @media (max-width: 480px) {
      .notif-container {
        left: 12px;
        right: 12px;
        width: auto;
        top: 12px;
      }
    }
  `]
})
export class NotificationComponent {
  public notificationService = inject(NotificationService);
}

