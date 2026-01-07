import { Component, HostListener, inject, OnInit, signal } from '@angular/core';
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
  public previewUrl = signal<string | null>(null);
  public isDragging = signal(false);
  public imageScale = signal(1);
  public imagePosition = signal({ x: 0, y: 0 });
  public isDraggingImage = signal(false);
  public dragStartPos = signal({ x: 0, y: 0 });

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
    this.previewUrl.set(null);
    this.resetImageAdjustments();
  }

  public cancelCreate(): void {
    this.isCreateMode.set(false);
    this.doctorForm.reset();
    this.selectedFile.set(null);
    this.previewUrl.set(null);
    this.resetImageAdjustments();
  }

  private resetImageAdjustments(): void {
    this.imageScale.set(1);
    this.imagePosition.set({ x: 0, y: 0 });
    this.isDraggingImage.set(false);
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.processFile(input.files[0]);
    }
  }

  public onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(true);
  }

  public onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(false);
  }

  public onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(false);

    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      const file = files[0];
      if (file.type.startsWith('image/')) {
        this.processFile(file);
      } else {
        this.notificationService.showError(
          this.translationService.currentLanguage() === 'uz' 
            ? 'Faqat rasm fayllarini yuklash mumkin' 
            : 'Можно загружать только изображения'
        );
      }
    }
  }

  private processFile(file: File): void {
    // Validate file type
    if (!file.type.startsWith('image/')) {
      this.notificationService.showError(
        this.translationService.currentLanguage() === 'uz' 
          ? 'Faqat rasm fayllarini yuklash mumkin' 
          : 'Можно загружать только изображения'
      );
      return;
    }

    // Validate file size (max 5MB)
    const maxSize = 5 * 1024 * 1024; // 5MB in bytes
    if (file.size > maxSize) {
      this.notificationService.showError(
        this.translationService.currentLanguage() === 'uz' 
          ? 'Rasm hajmi 5MB dan oshmasligi kerak' 
          : 'Размер изображения не должен превышать 5MB'
      );
      return;
    }

    this.selectedFile.set(file);

    // Create preview
    const reader = new FileReader();
    reader.onload = (e: ProgressEvent<FileReader>) => {
      this.previewUrl.set(e.target?.result as string);
    };
    reader.readAsDataURL(file);
  }

  public removeSelectedFile(): void {
    this.selectedFile.set(null);
    this.previewUrl.set(null);
    this.resetImageAdjustments();
  }

  public onImageMouseDown(event: MouseEvent): void {
    event.preventDefault();
    this.isDraggingImage.set(true);
    this.dragStartPos.set({
      x: event.clientX - this.imagePosition().x,
      y: event.clientY - this.imagePosition().y
    });
  }

  public onImageMouseMove(event: MouseEvent): void {
    if (!this.isDraggingImage()) return;
    
    event.preventDefault();
    const newX = event.clientX - this.dragStartPos().x;
    const newY = event.clientY - this.dragStartPos().y;
    
    this.imagePosition.set({ x: newX, y: newY });
  }

  public onImageMouseUp(): void {
    this.isDraggingImage.set(false);
  }

  @HostListener('document:mousemove', ['$event'])
  onDocumentMouseMove(event: MouseEvent): void {
    this.onImageMouseMove(event);
  }

  @HostListener('document:mouseup')
  onDocumentMouseUp(): void {
    this.onImageMouseUp();
  }

  public zoomIn(): void {
    const newScale = Math.min(this.imageScale() + 0.1, 3);
    this.imageScale.set(newScale);
  }

  public zoomOut(): void {
    const newScale = Math.max(this.imageScale() - 0.1, 0.5);
    this.imageScale.set(newScale);
  }

  public resetPosition(): void {
    this.imagePosition.set({ x: 0, y: 0 });
    this.imageScale.set(1);
  }

  public getImageTransform(): string {
    const pos = this.imagePosition();
    const scale = this.imageScale();
    return `translate(${pos.x}px, ${pos.y}px) scale(${scale})`;
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
      fullNameUz: formValue.fullNameUz,
      bioUz: formValue.bioUz,
      specialtyUz: formValue.specialtyUz,
      fullNameRu: formValue.fullNameRu,
      bioRu: formValue.bioRu,
      specialtyRu: formValue.specialtyRu,
      photo: this.selectedFile()!
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

