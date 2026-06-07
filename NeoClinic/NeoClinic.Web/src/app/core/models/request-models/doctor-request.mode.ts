export interface CreateDoctorRequest {
    fullNameUz: string;
    bioUz: string;
    specialtyUz: string;
    fullNameRu: string;
    bioRu: string;
    specialtyRu: string;
    photo: File;
}

export interface DeleteDoctorRequest {
    DoctorId: string;
}

export interface UpdateDoctorRequest {
    doctorId: string;
    fullNameUz: string;
    bioUz: string;
    specialtyUz: string;
    fullNameRu: string;
    bioRu: string;
    specialtyRu: string;
}

export interface UpdateDoctorPhotoRequest {
    doctorId: string;
    photo: File;
}

export interface GetDoctorsRequest {
    doctorId?: string;
}
