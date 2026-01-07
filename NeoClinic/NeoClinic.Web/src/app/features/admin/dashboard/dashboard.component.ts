import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);

  public stats = signal({
    futureAppointments: 0,
    doctors: 0,
    services: 0,
    mediaFiles: 0
  });

  public loading = signal(true);

  ngOnInit(): void {
    this.loadStats();
  }

  private loadStats(): void {
    const today = new Date();
    const threeMonthsFromNow = new Date();
    threeMonthsFromNow.setMonth(today.getMonth() + 3);

    Promise.all([
      this.apiService.getAppointmentsRequest({}).toPromise(),
      this.apiService.getDoctorsRequest({}).toPromise(),
      this.apiService.getServicesRequest({}).toPromise(),
      this.apiService.getMediaFilesRequest({}).toPromise()
    ]).then(([appointments, doctors, services, media]) => {
      // Filter future appointments (next 3 months)
      const futureAppointments = appointments?.filter(apt => {
        if (!apt.appointmentDate) return false;
        const aptDate = new Date(apt.appointmentDate);
        return aptDate >= today && aptDate <= threeMonthsFromNow;
      }) || [];

      this.stats.set({
        futureAppointments: futureAppointments.length,
        doctors: doctors?.length || 0,
        services: services?.length || 0,
        mediaFiles: media?.length || 0
      });
      this.loading.set(false);
    }).catch(error => {
      console.error('Failed to load stats:', error);
      this.loading.set(false);
    });
  }
}

