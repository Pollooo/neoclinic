import { MediaType } from "../enums/media-type";

export interface DeleteMediaFileRequest {
    FileId: string;
}

export interface GetMediaFilesRequest {}

export interface UploadMediaFileRequest {
    FileDescriptionUz?: string;
    FileDescriptionRu?: string;
    AltTextUz?: string;
    AltTextRu?: string;
    Type: MediaType;
    File: File;
}