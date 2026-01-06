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
    appointments: 0,
    messages: 0,
    doctors: 0,
    services: 0,
    mediaFiles: 0
  });

  public loading = signal(true);

  ngOnInit(): void {
    this.loadStats();
  }

  private loadStats(): void {
    Promise.all([
      this.apiService.getContactMessageRequest({}).toPromise(),
      this.apiService.getDoctorsRequest({}).toPromise(),
      this.apiService.getServicesRequest({}).toPromise(),
      this.apiService.getMediaFilesRequest({}).toPromise()
    ]).then(([messages, doctors, services, media]) => {
      this.stats.set({
        appointments: 0, // No GET endpoint for appointments
        messages: messages?.length || 0,
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

