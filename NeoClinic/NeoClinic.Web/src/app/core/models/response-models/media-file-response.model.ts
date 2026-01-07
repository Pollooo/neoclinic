export interface GetMediaFilesResponse {
    id: string;  // Changed from fileId to match backend response
    fileDescriptionUz?: string;
    fileDescriptionRu?: string;
    altTextUz?: string;
    altTextRu?: string;
    fileUrl: string;
    contentType?: string;
    type: number;  // Changed from string to number to match backend (0=Image, 1=Video)
}