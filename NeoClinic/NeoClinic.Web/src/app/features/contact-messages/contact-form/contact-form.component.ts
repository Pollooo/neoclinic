import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { CreateContactMessageRequest } from '../../../core/models/request-models/contact-message-request.model';

@Component({
  selector: 'app-contact-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './contact-form.component.html',
  styleUrl: './contact-form.component.css'
})
export class ContactFormComponent implements OnInit {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private formBuilder = inject(FormBuilder);
  public router = inject(Router);

  public submitting = signal(false);
  public contactForm!: FormGroup;

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.contactForm = this.formBuilder.group({
      fullName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required]],
      additionalPhoneNumber: [''],
      message: ['', [Validators.required]]
    });
  }

  public onSubmit(): void {
    if (this.contactForm.invalid) {
      this.contactForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);

    const formValue = this.contactForm.value;
    const request: CreateContactMessageRequest = {
      Name: formValue.fullName,
      Email: formValue.email,
      PhoneNumber: formValue.phoneNumber,
      AdditionalPhoneNumber: formValue.additionalPhoneNumber || undefined,
      AboutClinicUz: formValue.message,  // Storing user message in AboutClinicUz temporarily
      AboutClinicRu: formValue.message   // Storing user message in AboutClinicRu temporarily
    };

    this.apiService.createContactMessageRequest(request).subscribe({
      next: () => {
        this.notificationService.showSuccess(this.translationService.t('contact.success'));
        this.contactForm.reset();
        setTimeout(() => {
          this.router.navigate(['/']);
        }, 2000);
      },
      error: (error) => {
        this.notificationService.showError(error.message || this.translationService.t('contact.error'));
        this.submitting.set(false);
      },
      complete: () => {
        this.submitting.set(false);
      }
    });
  }

  public isFieldInvalid(fieldName: string): boolean {
    const field = this.contactForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  public getErrorMessage(fieldName: string): string {
    const lang = this.translationService.currentLanguage();
    const field = this.contactForm.get(fieldName);
    
    if (field?.hasError('required')) {
      return lang === 'uz' ? 'Bu maydon to\'ldirilishi shart' : 'Это поле обязательно';
    }
    if (field?.hasError('email')) {
      return lang === 'uz' ? 'Email manzil noto\'g\'ri' : 'Неверный email адрес';
    }
    return '';
  }
}

