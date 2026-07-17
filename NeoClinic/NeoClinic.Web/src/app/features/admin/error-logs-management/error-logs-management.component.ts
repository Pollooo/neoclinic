import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ErrorLogItem, GetErrorLogsResponse } from '../../../core/models/response-models/error-log-response.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-error-logs-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './error-logs-management.component.html',
  styleUrl: './error-logs-management.component.css'
})
export class ErrorLogsManagementComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);

  public logs = signal<ErrorLogItem[]>([]);
  public loading = signal(true);
  public page = signal(1);
  public pageSize = signal(10);
  public totalCount = signal(0);
  public totalPages = signal(0);
  public expandedLog = signal<string | null>(null);
  public statusFilter = signal<number | 'all'>('all');

  ngOnInit(): void {
    this.loadLogs();
  }

  public loadLogs(): void {
    this.loading.set(true);
    this.apiService.getErrorLogsRequest(this.page(), this.pageSize()).subscribe({
      next: (response: GetErrorLogsResponse) => {
        this.logs.set(response.items);
        this.totalCount.set(response.totalCount);
        this.totalPages.set(response.totalPages);
        this.page.set(response.page);
        this.loading.set(false);
      },
      error: () => {
        this.notificationService.showError(
          this.translationService.currentLanguage() === 'uz'
            ? 'Xatoliklar jurnalini yuklashda muammo yuz berdi'
            : 'Ошибка загрузки журнала ошибок'
        );
        this.loading.set(false);
      }
    });
  }

  public goToPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.page.set(p);
    this.loadLogs();
  }

  public toggleExpand(id: string): void {
    this.expandedLog.set(this.expandedLog() === id ? null : id);
  }

  public pageNumbers(): number[] {
    const total = this.totalPages();
    const current = this.page();
    const pages: number[] = [];
    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);
    for (let i = start; i <= end; i++) pages.push(i);
    return pages;
  }

  public statusBadgeClass(code: number): string {
    if (code >= 500) return 'badge-5xx';
    if (code >= 400) return 'badge-4xx';
    if (code >= 300) return 'badge-3xx';
    return 'badge-other';
  }

  public formatTimestamp(ts: string): string {
    const d = new Date(ts);
    return d.toLocaleString();
  }

  public truncateMessage(msg: string, len: number = 80): string {
    return msg.length > len ? msg.substring(0, len) + '...' : msg;
  }
}
