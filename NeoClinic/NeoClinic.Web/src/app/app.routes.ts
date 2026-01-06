import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Public routes
  {
    path: '',
    loadComponent: () => import('./features/public/public-layout.component').then(m => m.PublicLayoutComponent),
    children: [
      {
        path: '',
        loadComponent: () => import('./features/public/home/home.component').then(m => m.HomeComponent)
      },
      {
        path: 'appointment/create',
        loadComponent: () => import('./features/appointment/appointment-create/appointment-create.component').then(m => m.AppointmentCreateComponent)
      },
      {
        path: 'services',
        loadComponent: () => import('./features/service-managment/service-list/service-list.component').then(m => m.ServiceListComponent)
      },
      {
        path: 'doctors',
        loadComponent: () => import('./features/doctors/doctor-list/doctor-list.component').then(m => m.DoctorListComponent)
      },
      {
        path: 'contact',
        loadComponent: () => import('./features/contact-messages/contact-form/contact-form.component').then(m => m.ContactFormComponent)
      },
      {
        path: 'gallery',
        loadComponent: () => import('./features/media-file/gallery/gallery.component').then(m => m.GalleryComponent)
      }
    ]
  },
  
  // Admin routes
  {
    path: 'admin/login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'admin',
    loadComponent: () => import('./features/admin/admin-layout/admin-layout.component').then(m => m.AdminLayoutComponent),
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/admin/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'appointments',
        loadComponent: () => import('./features/admin/appointments-management/appointments-management.component').then(m => m.AppointmentsManagementComponent)
      },
      {
        path: 'messages',
        loadComponent: () => import('./features/admin/messages-management/messages-management.component').then(m => m.MessagesManagementComponent)
      },
      {
        path: 'doctors',
        loadComponent: () => import('./features/admin/doctors-management/doctors-management.component').then(m => m.DoctorsManagementComponent)
      },
      {
        path: 'services',
        loadComponent: () => import('./features/admin/services-management/services-management.component').then(m => m.ServicesManagementComponent)
      },
      {
        path: 'media',
        loadComponent: () => import('./features/admin/media-management/media-management.component').then(m => m.MediaManagementComponent)
      }
    ]
  },
  
  // Fallback route
  {
    path: '**',
    redirectTo: ''
  }
];
