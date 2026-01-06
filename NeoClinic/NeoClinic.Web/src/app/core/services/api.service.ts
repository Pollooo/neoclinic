import { inject, Injectable } from "@angular/core";
import { HttpService } from "./http.service";
import { LogInRequest } from "../models/request-models/log-in-request.model";
import { LogInResponse } from "../models/response-models/log-in-response.model";
import { Observable } from "rxjs";
import { routes } from "../../shared/routes";
import { CreateAppointmentRequest } from "../models/request-models/appointment-request.model";
import { CreateContactMessageRequest, GetContactMessageRequest, UpdateContactMessageRequest } from "../models/request-models/contact-message-request.model";
import { GetContactMessageResponse } from "../models/response-models/contact-message-response.model";
import { CreateDoctorRequest, DeleteDoctorRequest, GetDoctorsRequest } from "../models/request-models/doctor-request.mode";
import { GetDoctorsResponse } from "../models/response-models/doctor-response.model";
import { DeleteMediaFileRequest, GetMediaFilesRequest, UploadMediaFileRequest } from "../models/request-models/media-file-request.mode";
import { CreateServiceRequest, DeleteServiceRequest, GetServicesRequest } from "../models/request-models/service-request.modet";
import { GetServicesResponse } from "../models/response-models/service-response.model";
import { GetMediaFilesResponse } from "../models/response-models/media-file-response.model";

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

    public createContactMessageRequest(request: CreateContactMessageRequest): Observable<boolean> {
        return this.httpService.post<boolean>(routes.contact_messages.create, request);
    }

    public updateContactMessageRequest(request: UpdateContactMessageRequest): Observable<boolean> {
        return this.httpService.put<boolean>(routes.contact_messages.update, request);
    }

    public getContactMessageRequest(request: GetContactMessageRequest): Observable<GetContactMessageResponse[]> {
        return this.httpService.get<GetContactMessageResponse[]>(routes.contact_messages.get, request);
    }

    public createDoctorRequest(request: CreateDoctorRequest): Observable<boolean> {
        return this.httpService.post<boolean>(routes.doctors.create, request);
    }

    public deleteDoctorRequest(request: DeleteDoctorRequest): Observable<boolean> {
        return this.httpService.delete<boolean>(routes.doctors.delete(request.DoctorId));
    }

    public getDoctorsRequest(request: GetDoctorsRequest): Observable<GetDoctorsResponse[]> {
        return this.httpService.get<GetDoctorsResponse[]>(routes.doctors.get, request);
    }

    public uploadMediaFileRequest(request: UploadMediaFileRequest): Observable<boolean> {
        return this.httpService.post<boolean>(routes.media_files.upload, request);
    }

    public deleteMediaFileRequest(request: DeleteMediaFileRequest): Observable<boolean> {
        return this.httpService.delete<boolean>(routes.media_files.delete(request.FileId));
    }

    public getMediaFilesRequest(request: GetMediaFilesRequest): Observable<GetMediaFilesResponse[]> {
        return this.httpService.get<GetMediaFilesResponse[]>(routes.media_files.get, request);
    }

    public createServiceRequest(request: CreateServiceRequest): Observable<boolean> {
        return this.httpService.post<boolean>(routes.services.create, request);
    }

    public deleteServiceRequest(request: DeleteServiceRequest): Observable<boolean> {
        return this.httpService.delete<boolean>(routes.services.delete(request.ServiceId));
    }

    public getServicesRequest(request: GetServicesRequest): Observable<GetServicesResponse[]> {
        return this.httpService.get<GetServicesResponse[]>(routes.services.get, request);
    }
  } 