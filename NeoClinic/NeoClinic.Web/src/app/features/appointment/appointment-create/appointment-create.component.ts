import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GetServicesResponse } from '../../../core/models/response-models/service-response.model';

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
  public services = signal<GetServicesResponse[]>([]);
  public loadingServices = signal(true);
  public minDate = this.getTodayDate();

  public appointmentForm: FormGroup = this.formBuilder.group({
    fullName: ['', [Validators.required]],
    phoneNumber: ['+998 ', [Validators.required]],
    email: ['', [Validators.email]],
    serviceId: ['', [Validators.required]],
    appointmentDate: ['', [Validators.required]],
    description: ['']
  });

  private getTodayDate(): string {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  ngOnInit(): void {
    this.loadServices();
    this.setupPhoneNumberControl();
  }

  private setupPhoneNumberControl(): void {
    const phoneControl = this.appointmentForm.get('phoneNumber');
    
    phoneControl?.valueChanges.subscribe(value => {
      if (!value || !value.startsWith('+998')) {
        phoneControl.setValue('+998 ', { emitEvent: false });
      }
    });
  }

  public onPhoneNumberFocus(event: FocusEvent): void {
    const input = event.target as HTMLInputElement;
    const phoneControl = this.appointmentForm.get('phoneNumber');
    const value = phoneControl?.value || '';
    
    if (value === '+998 ' || value.length <= 5) {
      setTimeout(() => {
        input.setSelectionRange(5, 5);
      }, 0);
    }
  }

  private loadServices(): void {
    this.loadingServices.set(true);
    this.apiService.getServicesRequest({}).subscribe({
      next: (services) => {
        console.log('Services loaded:', services);
        console.log('Number of services:', services.length);
        if (services.length > 0) {
          console.log('First service sample:', services[0]);
          console.log('Service names:', services.map(s => ({ 
            id: s.serviceId, 
            nameUz: s.nameUz, 
            nameRu: s.nameRu 
          })));
        }
        this.services.set(services);
        this.loadingServices.set(false);
      },
      error: (error) => {
        console.error('Failed to load services:', error);
        this.notificationService.showError(error.message || 'Failed to load services');
        this.loadingServices.set(false);
      }
    });
  }

  public getServiceName(service: GetServicesResponse): string {
    const currentLang = this.translationService.currentLanguage();
    const name = currentLang === 'uz' ? service.nameUz : service.nameRu;
    
    // Fallback to the other language if current is empty
    if (!name || name.trim() === '') {
      return currentLang === 'uz' ? (service.nameRu || 'Unnamed Service') : (service.nameUz || 'Unnamed Service');
    }
    
    return name;
  }

  public onSubmit(): void {
    if (this.appointmentForm.invalid) {
      this.appointmentForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);

    const formValue = this.appointmentForm.value;
    
    // Ensure serviceId is set
    if (!formValue.serviceId) {
      this.notificationService.showError(this.translationService.currentLanguage() === 'uz' 
        ? 'Xizmatni tanlang' 
        : 'Выберите услугу');
      this.submitting.set(false);
      return;
    }

    const request = {
      patientName: formValue.fullName,
      phoneNumber: formValue.phoneNumber,
      email: formValue.email || undefined,
      serviceId: formValue.serviceId,
      appointmentDate: formValue.appointmentDate,
      message: formValue.description || undefined
    };

    console.log('Creating appointment with request:', request);

    this.apiService.createAppointmentRequest(request).subscribe({
      next: () => {
        this.notificationService.showSuccess(this.translationService.t('appointment.success'));
        this.appointmentForm.reset();
        this.appointmentForm.patchValue({ phoneNumber: '+998 ' });
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
