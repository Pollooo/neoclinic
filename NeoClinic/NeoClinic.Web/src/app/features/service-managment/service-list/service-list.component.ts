import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { GetServicesResponse } from '../../../core/models/response-models/service-response.model';

@Component({
  selector: 'app-service-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './service-list.component.html',
  styleUrl: './service-list.component.css'
})
export class ServiceListComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);

  public services = signal<GetServicesResponse[]>([]);
  public loading = signal(true);

  ngOnInit(): void {
    this.loadServices();
  }

  private loadServices(): void {
    this.apiService.getServicesRequest({}).subscribe({
      next: (services) => {
        this.services.set(services);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Failed to load services:', error);
        this.loading.set(false);
      }
    });
  }
}

