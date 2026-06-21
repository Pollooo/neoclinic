export interface CreateAppointmentRequest {
    patientName: string;
    phoneNumber: string;
    email?: string;
    message?: string;
    serviceId: string;
    appointmentDate: Date;
}

export interface GetAppointmentsRequest {
    startDate?: string;
    endDate?: string;
    serviceId?: string;
}

export interface DeleteAppointmentRequest {
    appointmentId: string;
}
