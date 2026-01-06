export const routes = {
    appointments: {
        create: 'appointments/create',
    },
    auth: {
        login: 'auth/login',
    },
    contact_messages: {
        create: 'contact-messages/create',
        update: 'contact-messages/update',
        get: 'contact-messages/get',
    },
    doctors: {
        create: 'doctors/create',
        delete: (doctorId: string) => `doctors/delete/${doctorId}`,
        get: 'doctors/get',
    },
    media_files: {
        upload: 'media-files/upload',
        delete: (fileId: string) => `media-files/delete/${fileId}`,
        get: 'media-files/get',
    },
    services: {
        create: 'services/create',
        delete: (serviceId: string) => `services/delete/${serviceId}`,
        get: 'services/get',
    },
}