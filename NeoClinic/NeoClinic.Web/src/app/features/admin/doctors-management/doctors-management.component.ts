import { Component, HostListener, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslationService } from '../../../core/services/translation.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { GetDoctorsResponse } from '../../../core/models/response-models/doctor-response.model';
import { environment } from '../../../environments/environment';
import { routes } from '../../../shared/routes';
import { from, Observable, of, switchMap } from 'rxjs';

/** Output size of the cropped square (px) sent to the server */
const CROP_SIZE = 600;

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
  public editingDoctor = signal<GetDoctorsResponse | null>(null);
  public submitting = signal(false);

  // ── Image adjuster state ─────────────────────────────────────────────
  /** The original File chosen by the user (never mutated). */
  public selectedFile = signal<File | null>(null);
  /** data-URL of the original file, shown in the <img> adjuster. */
  public previewUrl = signal<string | null>(null);
  /** True when a new file is selected (vs just showing existing photoUrl). */
  public hasNewFile = signal(false);

  public isDraggingZone = signal(false);

  // Pan & zoom applied to the preview image via CSS transform
  public imageScale = signal(1);
  public imageOffset = signal({ x: 0, y: 0 });

  // Internal drag tracking
  private _isDragging = false;
  private _dragStart = { x: 0, y: 0 };

  public doctorForm!: FormGroup;

  ngOnInit(): void {
    this.initializeForm();
    this.loadDoctors();
  }

  // ── Form ─────────────────────────────────────────────────────────────

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

  // ── Modal open / close ───────────────────────────────────────────────

  public openCreateMode(): void {
    this.isCreateMode.set(true);
    this.editingDoctor.set(null);
    this.doctorForm.reset();
    this.clearImage();
  }

  public openEditMode(doctor: GetDoctorsResponse): void {
    this.isCreateMode.set(true);
    this.editingDoctor.set(doctor);
    this.doctorForm.patchValue({
      fullNameUz: doctor.fullNameUz,
      bioUz: doctor.bioUz || '',
      specialtyUz: doctor.specialtyUz || '',
      fullNameRu: doctor.fullNameRu,
      bioRu: doctor.bioRu || '',
      specialtyRu: doctor.specialtyRu || ''
    });
    // Show existing photo (via proxy if blobName available) but mark as "not a new file"
    this.selectedFile.set(null);
    this.previewUrl.set(this.getDoctorPhotoUrl(doctor) || null);
    this.hasNewFile.set(false);
    this.resetAdjustments();
  }

  public cancelCreate(): void {
    this.isCreateMode.set(false);
    this.editingDoctor.set(null);
    this.doctorForm.reset();
    this.clearImage();
  }

  // ── Image state helpers ──────────────────────────────────────────────

  private clearImage(): void {
    this.selectedFile.set(null);
    this.previewUrl.set(null);
    this.hasNewFile.set(false);
    this.resetAdjustments();
  }

  private resetAdjustments(): void {
    this.imageScale.set(1);
    this.imageOffset.set({ x: 0, y: 0 });
    this._isDragging = false;
  }

  public removeSelectedFile(): void {
    this.clearImage();
  }

  // ── Drop-zone ────────────────────────────────────────────────────────

  public onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDraggingZone.set(true);
  }

  public onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDraggingZone.set(false);
  }

  public onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDraggingZone.set(false);
    const file = event.dataTransfer?.files?.[0];
    if (file) this.processFile(file);
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.[0]) this.processFile(input.files[0]);
  }

  private processFile(file: File): void {
    if (!file.type.startsWith('image/')) {
      this.notificationService.showError(
        this.translationService.currentLanguage() === 'uz'
          ? 'Faqat rasm fayllarini yuklash mumkin'
          : 'Можно загружать только изображения'
      );
      return;
    }
    if (file.size > 5 * 1024 * 1024) {
      this.notificationService.showError(
        this.translationService.currentLanguage() === 'uz'
          ? 'Rasm hajmi 5MB dan oshmasligi kerak'
          : 'Размер изображения не должен превышать 5MB'
      );
      return;
    }

    this.selectedFile.set(file);
    this.hasNewFile.set(true);
    this.resetAdjustments();

    const reader = new FileReader();
    reader.onload = (e) => this.previewUrl.set(e.target?.result as string);
    reader.readAsDataURL(file);
  }

  // ── Pan (drag image inside circle) ──────────────────────────────────

  public onMouseDown(event: MouseEvent): void {
    event.preventDefault();
    this._isDragging = true;
    this._dragStart = {
      x: event.clientX - this.imageOffset().x,
      y: event.clientY - this.imageOffset().y
    };
  }

  public onTouchStart(event: TouchEvent): void {
    if (event.touches.length !== 1) return;
    this._isDragging = true;
    this._dragStart = {
      x: event.touches[0].clientX - this.imageOffset().x,
      y: event.touches[0].clientY - this.imageOffset().y
    };
  }

  @HostListener('document:mousemove', ['$event'])
  onDocMouseMove(event: MouseEvent): void {
    if (!this._isDragging) return;
    event.preventDefault();
    this.imageOffset.set({
      x: event.clientX - this._dragStart.x,
      y: event.clientY - this._dragStart.y
    });
  }

  @HostListener('document:touchmove', ['$event'])
  onDocTouchMove(event: TouchEvent): void {
    if (!this._isDragging || event.touches.length !== 1) return;
    this.imageOffset.set({
      x: event.touches[0].clientX - this._dragStart.x,
      y: event.touches[0].clientY - this._dragStart.y
    });
  }

  @HostListener('document:mouseup')
  @HostListener('document:touchend')
  onDocMouseUp(): void {
    this._isDragging = false;
  }

  // ── Zoom ─────────────────────────────────────────────────────────────

  public onWheelZoom(event: WheelEvent): void {
    event.preventDefault();
    const delta = event.deltaY > 0 ? -0.05 : 0.05;
    this.changeScale(delta);
  }

  public zoomIn(): void { this.changeScale(+0.1); }
  public zoomOut(): void { this.changeScale(-0.1); }

  private changeScale(delta: number): void {
    const next = Math.min(4, Math.max(0.5, this.imageScale() + delta));
    this.imageScale.set(+next.toFixed(2));
  }

  public resetAdjust(): void {
    this.imageScale.set(1);
    this.imageOffset.set({ x: 0, y: 0 });
  }

  public getImageTransform(): string {
    const { x, y } = this.imageOffset();
    return `translate(${x}px, ${y}px) scale(${this.imageScale()})`;
  }

  public get scalePercent(): number {
    return Math.round(this.imageScale() * 100);
  }

  // ── Canvas crop → Blob ───────────────────────────────────────────────

  /**
   * Reads the current visual state (pan + zoom) and draws the corresponding
   * region of the original image onto an off-screen canvas, returning a File.
   *
   * The canvas is CROP_SIZE × CROP_SIZE px — what the server actually receives.
   */
  private cropToFile(): Promise<File> {
    return new Promise((resolve, reject) => {
      const url = this.previewUrl();
      if (!url) { reject(new Error('No image')); return; }

      const img = new Image();
      img.onload = () => {
        // Visible circle diameter in CSS pixels
        const circleDiameter = 240; // keep in sync with CSS .crop-circle size

        // Scale factor from natural image pixels → CSS display pixels
        // (the img fills the circle via object-fit:cover at scale=1)
        const naturalAspect = img.naturalWidth / img.naturalHeight;
        let cssImgW: number, cssImgH: number;
        if (naturalAspect >= 1) {
          cssImgH = circleDiameter;
          cssImgW = cssImgH * naturalAspect;
        } else {
          cssImgW = circleDiameter;
          cssImgH = cssImgW / naturalAspect;
        }

        const scale = this.imageScale();
        const offset = this.imageOffset();

        // The displayed image occupies a rect centered in the circle container,
        // shifted by offset and scaled from its own center.
        const displayW = cssImgW * scale;
        const displayH = cssImgH * scale;

        // Top-left of the displayed image relative to the circle's top-left
        const imgLeft = (circleDiameter - displayW) / 2 + offset.x;
        const imgTop = (circleDiameter - displayH) / 2 + offset.y;

        // We want to sample the circular crop region (0,0)→(circle,circle)
        // Map each circle pixel back to a natural image pixel:
        const scaleToNatW = img.naturalWidth / displayW;
        const scaleToNatH = img.naturalHeight / displayH;

        const srcX = (0 - imgLeft) * scaleToNatW;
        const srcY = (0 - imgTop) * scaleToNatH;
        const srcW = circleDiameter * scaleToNatW;
        const srcH = circleDiameter * scaleToNatH;

        const canvas = document.createElement('canvas');
        canvas.width = CROP_SIZE;
        canvas.height = CROP_SIZE;
        const ctx = canvas.getContext('2d')!;

        // Circular clip
        ctx.beginPath();
        ctx.arc(CROP_SIZE / 2, CROP_SIZE / 2, CROP_SIZE / 2, 0, Math.PI * 2);
        ctx.clip();

        ctx.drawImage(img, srcX, srcY, srcW, srcH, 0, 0, CROP_SIZE, CROP_SIZE);

        canvas.toBlob((blob) => {
          if (!blob) { reject(new Error('Canvas export failed')); return; }
          const original = this.selectedFile();
          const name = original ? original.name.replace(/\.[^.]+$/, '.jpg') : 'photo.jpg';
          resolve(new File([blob], name, { type: 'image/jpeg' }));
        }, 'image/jpeg', 0.92);
      };
      img.onerror = reject;
      img.src = url;
    });
  }

  // ── Save ─────────────────────────────────────────────────────────────

  public saveDoctor(): void {
    if (this.doctorForm.invalid) {
      this.doctorForm.markAllAsTouched();
      return;
    }

    const editingDoctor = this.editingDoctor();

    if (!editingDoctor && !this.selectedFile()) {
      this.notificationService.showWarning(
        this.translationService.currentLanguage() === 'uz'
          ? 'Rasm tanlang'
          : 'Выберите изображение'
      );
      return;
    }

    this.submitting.set(true);
    const formValue = this.doctorForm.value;

    // Decide whether we need to crop+upload a new photo
    const needPhoto = this.hasNewFile() && !!this.selectedFile();
    const photoPipeline$: Observable<File | null> = needPhoto
      ? from(this.cropToFile())
      : of(null);

    if (editingDoctor) {
      const textRequest = {
        doctorId: editingDoctor.doctorId,
        fullNameUz: formValue.fullNameUz,
        bioUz: formValue.bioUz,
        specialtyUz: formValue.specialtyUz,
        fullNameRu: formValue.fullNameRu,
        bioRu: formValue.bioRu,
        specialtyRu: formValue.specialtyRu
      };

      this.apiService.updateDoctorRequest(textRequest).pipe(
        switchMap(() => photoPipeline$),
        switchMap((croppedFile) =>
          croppedFile
            ? this.apiService.updateDoctorPhotoRequest({ doctorId: editingDoctor.doctorId, photo: croppedFile })
            : of(true)
        )
      ).subscribe({
        next: () => {
          this.notificationService.showSuccess(
            this.translationService.currentLanguage() === 'uz'
              ? 'Shifokor yangilandi'
              : 'Врач обновлён'
          );
          this.loadDoctors();
          this.cancelCreate();
        },
        error: (error) => {
          this.notificationService.showError(error.message);
          this.submitting.set(false);
        },
        complete: () => this.submitting.set(false)
      });
      return;
    }

    // Create flow — must have a photo
    photoPipeline$.pipe(
      switchMap((croppedFile) => {
        if (!croppedFile) throw new Error('No image selected');
        return this.apiService.createDoctorRequest({
          fullNameUz: formValue.fullNameUz,
          bioUz: formValue.bioUz,
          specialtyUz: formValue.specialtyUz,
          fullNameRu: formValue.fullNameRu,
          bioRu: formValue.bioRu,
          specialtyRu: formValue.specialtyRu,
          photo: croppedFile
        });
      })
    ).subscribe({
      next: () => {
        this.notificationService.showSuccess(
          this.translationService.currentLanguage() === 'uz'
            ? "Shifokor qo'shildi"
            : 'Врач добавлен'
        );
        this.loadDoctors();
        this.cancelCreate();
      },
      error: (error) => {
        this.notificationService.showError(error.message);
        this.submitting.set(false);
      },
      complete: () => this.submitting.set(false)
    });
  }

  // ── Delete ───────────────────────────────────────────────────────────

  public deleteDoctor(doctorId: string): void {
    if (!confirm(
      this.translationService.currentLanguage() === 'uz'
        ? "Shifokorni o'chirmoqchimisiz?"
        : 'Удалить врача?'
    )) return;

    this.apiService.deleteDoctorRequest({ DoctorId: doctorId }).subscribe({
      next: () => {
        this.notificationService.showSuccess(
          this.translationService.currentLanguage() === 'uz'
            ? "Shifokor o'chirildi"
            : 'Врач удалён'
        );
        this.loadDoctors();
      },
      error: (error) => this.notificationService.showError(error.message)
    });
  }

  // ── Display helpers ──────────────────────────────────────────────────

  public getDoctorName(doctor: GetDoctorsResponse): string {
    return this.translationService.currentLanguage() === 'uz'
      ? doctor.fullNameUz : doctor.fullNameRu;
  }

  public getDoctorSpecialty(doctor: GetDoctorsResponse): string {
    return this.translationService.currentLanguage() === 'uz'
      ? (doctor.specialtyUz || '') : (doctor.specialtyRu || '');
  }

  public getDoctorBio(doctor: GetDoctorsResponse): string {
    return this.translationService.currentLanguage() === 'uz'
      ? (doctor.bioUz || '') : (doctor.bioRu || '');
  }

  public getDoctorPhotoUrl(doctor: GetDoctorsResponse): string {
    if (doctor.blobName) {
      return `${environment.apiBaseUrl}/${routes.media_files.proxy(doctor.blobName)}`;
    }
    return doctor.photoUrl ?? '';
  }
}
