import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GetDoctorsResponse } from '../../../core/models/response-models/doctor-response.model';

@Component({
  selector: 'app-doctors-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './doctors-management.component.html',
  styleUrl: './doctors-management.component.css'
})
export class DoctorsManagementComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private formBuilder = inject(FormBuilder);

  public doctors = signal<GetDoctorsResponse[]>([]);
  public loading = signal(true);
  public isCreateMode = signal(false);
  public submitting = signal(false);
  public selectedFile = signal<File | null>(null);

  public doctorForm!: FormGroup;

  ngOnInit(): void {
    this.initializeForm();
    this.loadDoctors();
  }

  private initializeForm(): void {
    this.doctorForm = this.formBuilder.group({
      fullNameUz: ['', [Validators.required]],
      bioUz: ['', [Validators.required]],
      specialtyUz: ['', [Validators.required]],
      fullNameRu: ['', [Validators.required]],
      bioRu: ['', [Validators.required]],
      specialtyRu: ['', [Validators.required]]
    });
  }

  private loadDoctors(): void {
    this.apiService.getDoctorsRequest({}).subscribe({
      next: (doctors) => {
        this.doctors.set(doctors);
        this.loading.set(false);
      },
      error: (error) => {
        this.notificationService.showError(error.message);
        this.loading.set(false);
      }
    });
  }

  public openCreateMode(): void {
    this.isCreateMode.set(true);
    this.doctorForm.reset();
    this.selectedFile.set(null);
  }

  public cancelCreate(): void {
    this.isCreateMode.set(false);
    this.doctorForm.reset();
    this.selectedFile.set(null);
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.selectedFile.set(input.files[0]);
    }
  }

  public saveDoctor(): void {
    if (this.doctorForm.invalid) {
      this.doctorForm.markAllAsTouched();
      return;
    }

    if (!this.selectedFile()) {
      this.notificationService.showWarning(
        this.translationService.currentLanguage() === 'uz' 
          ? 'Rasm tanlang' 
          : 'Выберите изображение'
      );
      return;
    }

    this.submitting.set(true);
    const formValue = this.doctorForm.value;

    const request = {
      FullNameUz: formValue.fullNameUz,
      BioUz: formValue.bioUz,
      SpecialtyUz: formValue.specialtyUz,
      FullNameRu: formValue.fullNameRu,
      BioRu: formValue.bioRu,
      SpecialtyRu: formValue.specialtyRu,
      Photo: this.selectedFile()!
    };

    this.apiService.createDoctorRequest(request).subscribe({
      next: () => {
        this.notificationService.showSuccess(
          this.translationService.currentLanguage() === 'uz' 
            ? 'Shifokor qo\'shildi' 
            : 'Врач добавлен'
        );
        this.loadDoctors();
        this.cancelCreate();
      },
      error: (error) => {
        this.notificationService.showError(error.message);
      },
      complete: () => {
        this.submitting.set(false);
      }
    });
  }

  public deleteDoctor(doctorId: string): void {
    if (!confirm(this.translationService.currentLanguage() === 'uz' 
      ? 'Shifokorni o\'chirmoqchimisiz?' 
      : 'Удалить врача?')) {
      return;
    }

    this.apiService.deleteDoctorRequest({ DoctorId: doctorId }).subscribe({
      next: () => {
        this.notificationService.showSuccess(
          this.translationService.currentLanguage() === 'uz' 
            ? 'Shifokor o\'chirildi' 
            : 'Врач удален'
        );
        this.loadDoctors();
      },
      error: (error) => {
        this.notificationService.showError(error.message);
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
}

