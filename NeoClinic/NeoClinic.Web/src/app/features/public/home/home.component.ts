import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { GetServicesResponse } from '../../../core/models/response-models/service-response.model';
import { GetDoctorsResponse } from '../../../core/models/response-models/doctor-response.model';
import { GetContactMessageResponse } from '../../../core/models/response-models/contact-message-response.model';
import { GetMediaFilesResponse } from '../../../core/models/response-models/media-file-response.model';
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
  public contactInfo = signal<GetContactMessageResponse | null>(null);
  public backgroundImage = signal<string | null>(null);
  public galleryMedia = signal<GetMediaFilesResponse[]>([]);
  public galleryLoading = signal(true);

  ngOnInit(): void {
    this.loadServices();
    this.loadDoctors();
    this.loadContactInfo();
    this.loadBackgroundImage();
    this.loadGallery();
  }

  private loadContactInfo(): void {
    this.apiService.getContactMessageRequest({}).subscribe({
      next: (info) => {
        this.contactInfo.set(info);
      },
      error: (error) => {
        console.error('Failed to load contact info:', error);
      }
    });
  }

  private loadBackgroundImage(): void {
    this.apiService.getMediaFilesRequest({}).subscribe({
      next: (media) => {
        const backgroundMedia = media.find(m => 
          m.fileDescriptionUz?.toLowerCase().includes('background') || 
          m.fileDescriptionRu?.toLowerCase().includes('background')
        );
        if (backgroundMedia) {
          this.backgroundImage.set(backgroundMedia.fileUrl);
        }
      },
      error: (error) => {
        console.error('Failed to load background image:', error);
      }
    });
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
      ? doctor.fullNameUz 
      : doctor.fullNameRu;
  }

  public getDoctorSpecialty(doctor: GetDoctorsResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (doctor.specialtyUz || '') 
      : (doctor.specialtyRu || '');
  }

  public getDoctorBio(doctor: GetDoctorsResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (doctor.bioUz || '') 
      : (doctor.bioRu || '');
  }

  public getServiceName(service: GetServicesResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? service.nameUz 
      : service.nameRu;
  }

  public getServiceDescription(service: GetServicesResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (service.descriptionUz || '') 
      : (service.descriptionRu || '');
  }

  private loadGallery(): void {
    this.apiService.getMediaFilesRequest({}).subscribe({
      next: (media) => {
        // Filter: only images (type 0), exclude background, clinic name, and logo images, take only first 3
        const filteredMedia = media.filter(m => {
          const descUz = m.fileDescriptionUz?.toLowerCase() || '';
          const descRu = m.fileDescriptionRu?.toLowerCase() || '';
          return m.type === 0 && // Only images
                 !descUz.includes('background') && 
                 !descRu.includes('background') && 
                 !descUz.includes('clinic name') && 
                 !descRu.includes('clinic name') &&
                 !descUz.includes('logo') && 
                 !descRu.includes('logo');
        }).slice(0, 3);
        this.galleryMedia.set(filteredMedia);
        this.galleryLoading.set(false);
      },
      error: (error) => {
        console.error('Failed to load gallery:', error);
        this.galleryLoading.set(false);
      }
    });
  }
}
