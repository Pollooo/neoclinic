export interface CreateAppointmentRequest{
    PatientName: string;
    PhoneNumber: string;
    Email?: string;
    Message?: string;
    ServiceId: string;
    AppointmentDate: Date;
}