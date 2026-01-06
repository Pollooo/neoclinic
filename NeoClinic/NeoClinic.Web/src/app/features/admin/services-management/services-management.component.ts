import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GetServicesResponse } from '../../../core/models/response-models/service-response.model';

@Component({
  selector: 'app-services-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './services-management.component.html',
  styleUrl: './services-management.component.css'
})
export class ServicesManagementComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private formBuilder = inject(FormBuilder);

  public services = signal<GetServicesResponse[]>([]);
  public loading = signal(true);
  public isCreateMode = signal(false);
  public submitting = signal(false);

  public serviceForm!: FormGroup;

  ngOnInit(): void {
    this.initializeForm();
    this.loadServices();
  }

  private initializeForm(): void {
    this.serviceForm = this.formBuilder.group({
      nameUz: ['', [Validators.required]],
      descriptionUz: [''],
      nameRu: ['', [Validators.required]],
      descriptionRu: [''],
      price: ['', [Validators.min(0)]]
    });
  }

  private loadServices(): void {
    this.apiService.getServicesRequest({}).subscribe({
      next: (services) => {
        this.services.set(services);
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
    this.serviceForm.reset();
  }

  public cancelCreate(): void {
    this.isCreateMode.set(false);
    this.serviceForm.reset();
  }

  public saveService(): void {
    if (this.serviceForm.invalid) {
      this.serviceForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const formValue = this.serviceForm.value;

    const request = {
      NameUz: formValue.nameUz,
      DescriptionUz: formValue.descriptionUz || undefined,
      NameRu: formValue.nameRu,
      DescriptionRu: formValue.descriptionRu || undefined,
      Price: formValue.price ? Number(formValue.price) : undefined
    };

    this.apiService.createServiceRequest(request).subscribe({
      next: () => {
        this.notificationService.showSuccess(
          this.translationService.currentLanguage() === 'uz' 
            ? 'Xizmat qo\'shildi' 
            : 'Услуга добавлена'
        );
        this.loadServices();
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

  public deleteService(serviceId: string): void {
    if (!confirm(this.translationService.currentLanguage() === 'uz' 
      ? 'Xizmatni o\'chirmoqchimisiz?' 
      : 'Удалить услугу?')) {
      return;
    }

    this.apiService.deleteServiceRequest({ ServiceId: serviceId }).subscribe({
      next: () => {
        this.notificationService.showSuccess(
          this.translationService.currentLanguage() === 'uz' 
            ? 'Xizmat o\'chirildi' 
            : 'Услуга удалена'
        );
        this.loadServices();
      },
      error: (error) => {
        this.notificationService.showError(error.message);
      }
    });
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

