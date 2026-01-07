import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GetAppointmentResponse } from '../../../core/models/response-models/appointment-response.model';

@Component({
  selector: 'app-appointments-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './appointments-management.component.html',
  styleUrl: './appointments-management.component.css'
})
export class AppointmentsManagementComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);

  public appointments = signal<GetAppointmentResponse[]>([]);
  public loading = signal(true);
  public showFilters = signal(false);
  public filterStartDate = '';
  public filterEndDate = '';
  public filterServiceId = '';

  ngOnInit(): void {
    this.loadAppointments();
  }

  public toggleFilters(): void {
    this.showFilters.update(value => !value);
  }

  private loadAppointments(): void {
    this.loading.set(true);
    this.apiService.getAppointmentsRequest({
      startDate: this.filterStartDate || undefined,
      endDate: this.filterEndDate || undefined,
      serviceId: this.filterServiceId || undefined
    }).subscribe({
      next: (appointments) => {
        console.log('Appointments received:', appointments);
        this.appointments.set(appointments || []);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Failed to load appointments:', error);
        this.notificationService.showError(error.message);
        this.loading.set(false);
      }
    });
  }

  public applyFilter(): void {
    this.loadAppointments();
  }

  public clearFilter(): void {
    this.filterStartDate = '';
    this.filterEndDate = '';
    this.filterServiceId = '';
    this.loadAppointments();
  }

  public formatDate(dateString?: string): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    
    if (this.translationService.currentLanguage() === 'uz') {
      const months = [
        'Yanvar', 'Fevral', 'Mart', 'Aprel', 'May', 'Iyun',
        'Iyul', 'Avgust', 'Sentabr', 'Oktabr', 'Noyabr', 'Dekabr'
      ];
      const day = date.getDate();
      const month = months[date.getMonth()];
      const year = date.getFullYear();
      const hours = date.getHours().toString().padStart(2, '0');
      const minutes = date.getMinutes().toString().padStart(2, '0');
      return `${day} ${month} ${year} yil, ${hours}:${minutes}`;
    } else {
      return date.toLocaleDateString('ru-RU', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    }
  }

  public isAppointmentOld(dateString?: string): boolean {
    if (!dateString) return false;
    const appointmentDate = new Date(dateString);
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Reset time to midnight for accurate comparison
    appointmentDate.setHours(0, 0, 0, 0);
    return appointmentDate < today;
  }

  public getServiceName(service: any): string {
    if (!service) return '-';
    return this.translationService.currentLanguage() === 'uz' 
      ? (service.nameUz || '-')
      : (service.nameRu || '-');
  }
}

