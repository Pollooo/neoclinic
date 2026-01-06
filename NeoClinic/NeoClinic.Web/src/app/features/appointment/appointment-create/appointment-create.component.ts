import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GetDoctorsResponse } from '../../../core/models/response-models/doctor-response.model';

@Component({
  selector: 'app-appointment-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './appointment-create.component.html',
  styleUrl: './appointment-create.component.css'
})
export class AppointmentCreateComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private formBuilder = inject(FormBuilder);
  public router = inject(Router);

  public submitting = signal(false);
  public doctors = signal<GetDoctorsResponse[]>([]);

  public appointmentForm: FormGroup = this.formBuilder.group({
    fullName: ['', [Validators.required]],
    phoneNumber: ['', [Validators.required]],
    doctorId: [''],
    appointmentDate: ['', [Validators.required]],
    description: ['']
  });

  ngOnInit(): void {
    this.loadDoctors();
  }

  private loadDoctors(): void {
    this.apiService.getDoctorsRequest({}).subscribe({
      next: (doctors) => {
        this.doctors.set(doctors);
      },
      error: (error) => {
        console.error('Failed to load doctors:', error);
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

  public onSubmit(): void {
    if (this.appointmentForm.invalid) {
      this.appointmentForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);

    const formValue = this.appointmentForm.value;
    const request = {
      PatientName: formValue.fullName,
      PhoneNumber: formValue.phoneNumber,
      ServiceId: formValue.doctorId || '',  // Using doctorId as ServiceId temporarily
      AppointmentDate: formValue.appointmentDate,
      Message: formValue.description || undefined
    };

    this.apiService.createAppointmentRequest(request).subscribe({
      next: () => {
        this.notificationService.showSuccess(this.translationService.t('appointment.success'));
        this.appointmentForm.reset();
        setTimeout(() => {
          this.router.navigate(['/']);
        }, 2000);
      },
      error: (error) => {
        this.notificationService.showError(error.message || this.translationService.t('appointment.error'));
        this.submitting.set(false);
      },
      complete: () => {
        this.submitting.set(false);
      }
    });
  }
}
