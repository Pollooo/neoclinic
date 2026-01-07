import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GetContactMessageResponse } from '../../../core/models/response-models/contact-message-response.model';
import { CreateContactMessageRequest, UpdateContactMessageRequest } from '../../../core/models/request-models/contact-message-request.model';

@Component({
  selector: 'app-messages-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './messages-management.component.html',
  styleUrl: './messages-management.component.css'
})
export class MessagesManagementComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private formBuilder = inject(FormBuilder);

  public contactInfo = signal<GetContactMessageResponse | null>(null);
  public loading = signal(true);
  public isEditMode = signal(false);
  public isCreateMode = signal(false);
  public submitting = signal(false);

  public contactForm!: FormGroup;

  ngOnInit(): void {
    this.initializeForm();
    this.loadContactInfo();
  }

  private initializeForm(): void {
    this.contactForm = this.formBuilder.group({
      name: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required]],
      additionalPhoneNumber: [''],
      telegramChatUrl: [''],
      telegramUrl: [''],
      instagramUrl: [''],
      facebookUrl: [''],
      locationUrl: [''],
      aboutClinicUz: ['', [Validators.required]],
      aboutClinicRu: ['', [Validators.required]]
    });
  }

  private loadContactInfo(): void {
    this.loading.set(true);
    this.apiService.getContactMessageRequest({}).subscribe({
      next: (response) => {
        console.log('Admin contact info received:', response);
        this.contactInfo.set(response || null);
        
        if (response) {
          this.contactForm.patchValue({
            name: response.name || '',
            email: response.email || '',
            phoneNumber: response.phoneNumber || '',
            additionalPhoneNumber: response.additionalPhoneNumber || '',
            telegramChatUrl: response.telegramChatUrl || '',
            telegramUrl: response.telegramUrl || '',
            instagramUrl: response.instagramUrl || '',
            facebookUrl: response.facebookUrl || '',
            locationUrl: response.locationUrl || '',
            aboutClinicUz: response.aboutClinicUz || '',
            aboutClinicRu: response.aboutClinicRu || ''
          });
        }
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Failed to load admin contact info:', error);
        this.notificationService.showError(error.message);
        this.loading.set(false);
      }
    });
  }

  public toggleEditMode(): void {
    this.isEditMode.update(value => !value);
    if (!this.isEditMode()) {
      // Reset form to original values
      this.loadContactInfo();
    }
  }

  public startCreateMode(): void {
    this.isCreateMode.set(true);
    this.contactForm.reset();
  }

  public cancelCreateMode(): void {
    this.isCreateMode.set(false);
    this.contactForm.reset();
  }

  public saveContactInfo(): void {
    if (this.contactForm.invalid) {
      this.contactForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const formValue = this.contactForm.value;

    if (this.isCreateMode()) {
      // Create new contact info
      const createRequest: CreateContactMessageRequest = {
        name: formValue.name,
        email: formValue.email,
        phoneNumber: formValue.phoneNumber,
        additionalPhoneNumber: formValue.additionalPhoneNumber || undefined,
        telegramChatUrl: formValue.telegramChatUrl || undefined,
        telegramUrl: formValue.telegramUrl || undefined,
        instagramUrl: formValue.instagramUrl || undefined,
        facebookUrl: formValue.facebookUrl || undefined,
        locationUrl: formValue.locationUrl || undefined,
        aboutClinicUz: formValue.aboutClinicUz,
        aboutClinicRu: formValue.aboutClinicRu
      };

      this.apiService.createContactMessageRequest(createRequest).subscribe({
        next: () => {
          this.notificationService.showSuccess(
            this.translationService.currentLanguage() === 'uz' 
              ? 'Aloqa ma\'lumotlari yaratildi' 
              : 'Контактная информация создана'
          );
          this.loadContactInfo();
          this.isCreateMode.set(false);
        },
        error: (error) => {
          this.notificationService.showError(error.message);
        },
        complete: () => {
          this.submitting.set(false);
        }
      });
    } else {
      // Update existing contact info
      const updateRequest: UpdateContactMessageRequest = {
        name: formValue.name,
        email: formValue.email,
        phoneNumber: formValue.phoneNumber,
        additionalPhoneNumber: formValue.additionalPhoneNumber || undefined,
        telegramChatUrl: formValue.telegramChatUrl || undefined,
        telegramUrl: formValue.telegramUrl || undefined,
        instagramUrl: formValue.instagramUrl || undefined,
        facebookUrl: formValue.facebookUrl || undefined,
        locationUrl: formValue.locationUrl || undefined,
        aboutClinicUz: formValue.aboutClinicUz,
        aboutClinicRu: formValue.aboutClinicRu
      };

      this.apiService.updateContactMessageRequest(updateRequest).subscribe({
        next: () => {
          this.notificationService.showSuccess(
            this.translationService.currentLanguage() === 'uz' 
              ? 'Aloqa ma\'lumotlari yangilandi' 
              : 'Контактная информация обновлена'
          );
          this.loadContactInfo();
          this.isEditMode.set(false);
        },
        error: (error) => {
          this.notificationService.showError(error.message);
        },
        complete: () => {
          this.submitting.set(false);
        }
      });
    }
  }
}

