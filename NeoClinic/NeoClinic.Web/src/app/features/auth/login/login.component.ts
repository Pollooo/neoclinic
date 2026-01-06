import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  public translationService = inject(TranslationService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private localStorageService = inject(LocalStorageService);
  private formBuilder = inject(FormBuilder);
  private router = inject(Router);

  public submitting = signal(false);
  public loginForm: FormGroup = this.formBuilder.group({
    username: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  public onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);

    const formValue = this.loginForm.value;
    const request = {
      Username: formValue.username,
      Password: formValue.password
    };

    this.apiService.loginRequest(request).subscribe({
      next: (response) => {
        this.localStorageService.setAccessToken(response.AccessToken);
        this.notificationService.showSuccess(
          this.translationService.currentLanguage() === 'uz' 
            ? 'Tizimga muvaffaqiyatli kirdingiz' 
            : 'Вы успешно вошли в систему'
        );
        this.router.navigate(['/admin/dashboard']);
      },
      error: (error) => {
        this.notificationService.showError(error.message || this.translationService.t('admin.invalidCredentials'));
        this.submitting.set(false);
      },
      complete: () => {
        this.submitting.set(false);
      }
    });
  }

  public isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }
}

