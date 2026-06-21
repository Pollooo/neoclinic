export interface CreateServiceRequest {
    nameUz: string;
    descriptionUz?: string;
    nameRu: string;
    descriptionRu?: string;
    price?: number;
}

export interface DeleteServiceRequest {
    ServiceId: string;
}

export interface GetServicesRequest {
    serviceId?: string;
}

export interface UpdateServiceRequest {
    id: string;
    nameUz: string;
    descriptionUz?: string;
    nameRu: string;
    descriptionRu?: string;
    price?: number;
}
