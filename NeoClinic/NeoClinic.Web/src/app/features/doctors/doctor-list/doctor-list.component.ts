import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { GetDoctorsResponse } from '../../../core/models/response-models/doctor-response.model';

@Component({
  selector: 'app-doctor-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './doctor-list.component.html',
  styleUrl: './doctor-list.component.css'
})
export class DoctorListComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);

  public doctors = signal<GetDoctorsResponse[]>([]);
  public loading = signal(true);

  ngOnInit(): void {
    this.loadDoctors();
  }

  private loadDoctors(): void {
    this.apiService.getDoctorsRequest({}).subscribe({
      next: (doctors) => {
        this.doctors.set(doctors);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Failed to load doctors:', error);
        this.loading.set(false);
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
}
