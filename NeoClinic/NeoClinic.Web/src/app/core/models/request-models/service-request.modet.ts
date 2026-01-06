export interface CreateServiceRequest {
    NameUz: string;
    DescriptionUz?: string;
    NameRu: string;
    DescriptionRu?: string;
    Price?: number;
}

export interface DeleteServiceRequest {
    ServiceId: string;
}

export interface GetServicesRequest {
    ServiceId?: string;
}