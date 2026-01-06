export interface CreateDoctorRequest {
    FullNameUz: string;
    BioUz: string;
    SpecialtyUz: string;
    FullNameRu: string;
    BioRu: string;
    SpecialtyRu: string;
    Photo: File;
}

export interface DeleteDoctorRequest {
    DoctorId: string;
}

export interface GetDoctorsRequest {
    doctorId?: string;
}