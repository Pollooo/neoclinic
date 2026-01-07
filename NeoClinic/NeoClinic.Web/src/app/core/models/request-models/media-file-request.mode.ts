import { MediaType } from "../enums/media-type";

export interface DeleteMediaFileRequest {
    FileId: string;
}

export interface GetMediaFilesRequest {
    fileId?: string;
}

export interface UploadMediaFileRequest {
    fileDescriptionUz?: string;
    fileDescriptionRu?: string;
    altTextUz?: string;
    altTextRu?: string;
    type: MediaType;
    file: File;
}