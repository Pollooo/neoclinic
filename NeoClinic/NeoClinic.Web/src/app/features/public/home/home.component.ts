import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { GetServicesResponse } from '../../../core/models/response-models/service-response.model';
import { GetDoctorsResponse } from '../../../core/models/response-models/doctor-response.model';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);

  public services = signal<GetServicesResponse[]>([]);
  public servicesLoading = signal(true);
  public doctors = signal<GetDoctorsResponse[]>([]);
  public doctorsLoading = signal(true);

  ngOnInit(): void {
    this.loadServices();
    this.loadDoctors();
  }

  private loadServices(): void {
    this.apiService.getServicesRequest({}).subscribe({
      next: (services) => {
        this.services.set(services);
        this.servicesLoading.set(false);
      },
      error: (error) => {
        console.error('Failed to load services:', error);
        this.servicesLoading.set(false);
      }
    });
  }

  private loadDoctors(): void {
    this.apiService.getDoctorsRequest({}).subscribe({
      next: (doctors) => {
        this.doctors.set(doctors);
        this.doctorsLoading.set(false);
      },
      error: (error) => {
        console.error('Failed to load doctors:', error);
        this.doctorsLoading.set(false);
      }
    });
  }

  public getDoctorName(doctor: GetDoctorsResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? doctor.FullNameUz 
      : doctor.FullNameRu;
  }

  public getDoctorSpecialty(doctor: GetDoctorsResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (doctor.SpecialtyUz || '') 
      : (doctor.SpecialtyRu || '');
  }

  public getDoctorBio(doctor: GetDoctorsResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (doctor.BioUz || '') 
      : (doctor.BioRu || '');
  }

  public getServiceName(service: GetServicesResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? service.NameUz 
      : service.NameRu;
  }

  public getServiceDescription(service: GetServicesResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (service.DescriptionUz || '') 
      : (service.DescriptionRu || '');
  }
}
