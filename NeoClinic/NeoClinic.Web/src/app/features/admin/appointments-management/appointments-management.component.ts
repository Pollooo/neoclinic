import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslationService } from '../../../core/services/translation.service';

@Component({
  selector: 'app-appointments-management',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './appointments-management.component.html',
  styleUrl: './appointments-management.component.css'
})
export class AppointmentsManagementComponent {
  public translationService = inject(TranslationService);
}

