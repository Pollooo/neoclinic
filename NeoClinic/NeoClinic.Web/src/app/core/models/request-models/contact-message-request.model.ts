export interface CreateContactMessageRequest {
    Name?: string;
    Email?: string;
    PhoneNumber?: string;
    AdditionalPhoneNumber?: string;
    TelegramChatUrl?: string;
    TelegramUrl?: string;
    InstagramUrl?: string;
    FacebookUrl?: string;
    LocationUrl?: string;
    AboutClinicUz?: string;
    AboutClinicRu?: string;
}

export interface GetContactMessageRequest {}

export interface UpdateContactMessageRequest {
    Id?: string;
    Name?: string;
    Email?: string;
    PhoneNumber?: string;
    AdditionalPhoneNumber?: string;
    TelegramChatUrl?: string;
    TelegramUrl?: string;
    InstagramUrl?: string;
    FacebookUrl?: string;
    LocationUrl?: string;
    AboutClinicUz?: string;
    AboutClinicRu?: string;
}
