export interface GetServiceResponse {
    serviceId?: string;
    nameUz?: string;
    descriptionUz?: string;
    nameRu?: string;
    descriptionRu?: string;
    price?: number;
}

export interface GetAppointmentResponse {
    appointmentId?: string;
    patientName?: string;
    phoneNumber?: string;
    email?: string;
    message?: string;
    appointmentDate?: string;
    getServicesResponse?: GetServiceResponse;
    createdAt?: string;
}

