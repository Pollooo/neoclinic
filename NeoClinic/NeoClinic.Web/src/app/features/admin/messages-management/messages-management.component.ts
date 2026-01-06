import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GetContactMessageResponse } from '../../../core/models/response-models/contact-message-response.model';
import { UpdateContactMessageRequest } from '../../../core/models/request-models/contact-message-request.model';

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

  public messages = signal<GetContactMessageResponse[]>([]);
  public loading = signal(true);
  public selectedMessage = signal<GetContactMessageResponse | null>(null);
  public isEditMode = signal(false);
  public submitting = signal(false);

  public messageForm!: FormGroup;

  ngOnInit(): void {
    this.initializeForm();
    this.loadMessages();
  }

  private initializeForm(): void {
    this.messageForm = this.formBuilder.group({
      fullName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required]],
      additionalPhoneNumber: [''],
      message: ['', [Validators.required]]
    });
  }

  private loadMessages(): void {
    this.apiService.getContactMessageRequest({}).subscribe({
      next: (messages) => {
        this.messages.set(messages);
        this.loading.set(false);
      },
      error: (error) => {
        this.notificationService.showError(error.message);
        this.loading.set(false);
      }
    });
  }

  public getMessage(message: GetContactMessageResponse): string {
    return this.translationService.currentLanguage() === 'uz' 
      ? (message.AboutClinicUz || '') 
      : (message.AboutClinicRu || '');
  }

  public editMessage(message: GetContactMessageResponse): void {
    this.selectedMessage.set(message);
    this.isEditMode.set(true);
    this.messageForm.patchValue({
      fullName: message.Name,
      email: message.Email,
      phoneNumber: message.PhoneNumber,
      additionalPhoneNumber: message.AdditionalPhoneNumber || '',
      message: this.getMessage(message)
    });
  }

  public cancelEdit(): void {
    this.selectedMessage.set(null);
    this.isEditMode.set(false);
    this.messageForm.reset();
  }

  public saveMessage(): void {
    if (this.messageForm.invalid || !this.selectedMessage()) {
      return;
    }

    this.submitting.set(true);
    const formValue = this.messageForm.value;

    const request: UpdateContactMessageRequest = {
      Id: this.selectedMessage()!.Id,
      Name: formValue.fullName,
      Email: formValue.email,
      PhoneNumber: formValue.phoneNumber,
      AdditionalPhoneNumber: formValue.additionalPhoneNumber || undefined,
      AboutClinicUz: formValue.message,
      AboutClinicRu: formValue.message
    };

    this.apiService.updateContactMessageRequest(request).subscribe({
      next: () => {
        this.notificationService.showSuccess(
          this.translationService.currentLanguage() === 'uz' 
            ? 'Xabar yangilandi' 
            : 'Сообщение обновлено'
        );
        this.loadMessages();
        this.cancelEdit();
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

