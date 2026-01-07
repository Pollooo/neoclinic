export interface CreateContactMessageRequest {
    name?: string;
    email?: string;
    phoneNumber?: string;
    additionalPhoneNumber?: string;
    telegramChatUrl?: string;
    telegramUrl?: string;
    instagramUrl?: string;
    facebookUrl?: string;
    locationUrl?: string;
    aboutClinicUz?: string;
    aboutClinicRu?: string;
}

export interface GetContactMessageRequest {}

export interface UpdateContactMessageRequest {
    name?: string;
    email?: string;
    phoneNumber?: string;
    additionalPhoneNumber?: string;
    telegramChatUrl?: string;
    telegramUrl?: string;
    instagramUrl?: string;
    facebookUrl?: string;
    locationUrl?: string;
    aboutClinicUz?: string;
    aboutClinicRu?: string;
}