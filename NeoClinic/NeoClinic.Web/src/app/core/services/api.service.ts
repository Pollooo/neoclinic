import { inject, Injectable } from "@angular/core";
import { HttpService } from "./http.service";
import { LogInRequest } from "../models/request-models/log-in-request.model";
import { LogInResponse } from "../models/response-models/log-in-response.model";
import { Observable } from "rxjs";
import { routes } from "../../shared/routes";
import { CreateAppointmentRequest, GetAppointmentsRequest } from "../models/request-models/appointment-request.model";
import { GetAppointmentResponse } from "../models/response-models/appointment-response.model";
import { CreateContactMessageRequest, GetContactMessageRequest, UpdateContactMessageRequest } from "../models/request-models/contact-message-request.model";
import { GetContactMessageResponse } from "../models/response-models/contact-message-response.model";
import { CreateDoctorRequest, DeleteDoctorRequest, GetDoctorsRequest } from "../models/request-models/doctor-request.mode";
import { GetDoctorsResponse } from "../models/response-models/doctor-response.model";
import { DeleteMediaFileRequest, GetMediaFilesRequest, UploadMediaFileRequest } from "../models/request-models/media-file-request.mode";
import { CreateServiceRequest, DeleteServiceRequest, GetServicesRequest } from "../models/request-models/service-request.modet";
import { GetServicesResponse } from "../models/response-models/service-response.model";
import { GetMediaFilesResponse } from "../models/response-models/media-file-response.model";
import { HttpParams } from "@angular/common/http";

@Injectable({
    providedIn: 'root'
  })
  export class ApiService {
    private readonly httpService = inject(HttpService);

    public loginRequest(request: LogInRequest): Observable<LogInResponse> {
        return this.httpService.post<LogInResponse>(routes.auth.login, request);
    }

    public createAppointmentRequest(request: CreateAppointmentRequest): Observable<boolean> {
        return this.httpService.post<boolean>(routes.appointments.create, request);
    }

    public getAppointmentsRequest(request: GetAppointmentsRequest): Observable<GetAppointmentResponse[]> {
        let params = new HttpParams();
        if (request.startDate) {
            params = params.set('startDate', request.startDate);
        }
        if (request.endDate) {
            params = params.set('endDate', request.endDate);
        }
        if (request.serviceId) {
            params = params.set('serviceId', request.serviceId);
        }
        return this.httpService.get<GetAppointmentResponse[]>(routes.appointments.get, { params });
    }

    public createContactMessageRequest(request: CreateContactMessageRequest): Observable<boolean> {
        return this.httpService.post<boolean>(routes.contact_messages.create, request);
    }

    public updateContactMessageRequest(request: UpdateContactMessageRequest): Observable<boolean> {
        return this.httpService.put<boolean>(routes.contact_messages.update, request);
    }

    public getContactMessageRequest(request: GetContactMessageRequest): Observable<GetContactMessageResponse> {
        return this.httpService.get<GetContactMessageResponse>(routes.contact_messages.get);
    }

    public createDoctorRequest(request: CreateDoctorRequest): Observable<boolean> {
        const formData = new FormData();
        formData.append('FullNameUz', request.fullNameUz);
        formData.append('BioUz', request.bioUz);
        formData.append('SpecialtyUz', request.specialtyUz);
        formData.append('FullNameRu', request.fullNameRu);
        formData.append('BioRu', request.bioRu);
        formData.append('SpecialtyRu', request.specialtyRu);
        formData.append('Photo', request.photo, request.photo.name);
        
        return this.httpService.post<boolean>(routes.doctors.create, formData);
    }

    public deleteDoctorRequest(request: DeleteDoctorRequest): Observable<boolean> {
        return this.httpService.delete<boolean>(routes.doctors.delete(request.DoctorId));
    }

    public getDoctorsRequest(request: GetDoctorsRequest): Observable<GetDoctorsResponse[]> {
        let params = new HttpParams();
        if (request.doctorId) {
            params = params.set('doctorId', request.doctorId);
        }
        return this.httpService.get<GetDoctorsResponse[]>(routes.doctors.get, { params });
    }

    public uploadMediaFileRequest(request: UploadMediaFileRequest): Observable<boolean> {
        const formData = new FormData();
        if (request.fileDescriptionUz) {
            formData.append('FileDescriptionUz', request.fileDescriptionUz);
        }
        if (request.fileDescriptionRu) {
            formData.append('FileDescriptionRu', request.fileDescriptionRu);
        }
        if (request.altTextUz) {
            formData.append('AltTextUz', request.altTextUz);
        }
        if (request.altTextRu) {
            formData.append('AltTextRu', request.altTextRu);
        }
        formData.append('Type', request.type.toString());
        formData.append('File', request.file, request.file.name);
        
        return this.httpService.post<boolean>(routes.media_files.upload, formData);
    }

    public deleteMediaFileRequest(request: DeleteMediaFileRequest): Observable<boolean> {
        return this.httpService.delete<boolean>(routes.media_files.delete(request.FileId));
    }

    public getMediaFilesRequest(request: GetMediaFilesRequest): Observable<GetMediaFilesResponse[]> {
        let params = new HttpParams();
        if (request.fileId) {
            params = params.set('fileId', request.fileId);
        }
        return this.httpService.get<GetMediaFilesResponse[]>(routes.media_files.get, { params });
    }

    public createServiceRequest(request: CreateServiceRequest): Observable<boolean> {
        return this.httpService.post<boolean>(routes.services.create, request);
    }

    public deleteServiceRequest(request: DeleteServiceRequest): Observable<boolean> {
        return this.httpService.delete<boolean>(routes.services.delete(request.ServiceId));
    }

    public getServicesRequest(request: GetServicesRequest): Observable<GetServicesResponse[]> {
        let params = new HttpParams();
        if (request.serviceId) {
            params = params.set('serviceId', request.serviceId);
        }
        return this.httpService.get<GetServicesResponse[]>(routes.services.get, { params });
    }
  } 