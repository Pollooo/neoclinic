export const routes = {
    appointments: {
        create: 'appointments/create',
        delete: (appointmentId: string) => `appointments/delete/${appointmentId}`,
        get: 'appointments/get',
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
        update: 'doctors/update',
        updatePhoto: 'doctors/update-photo',
        get: 'doctors/get',
    },
    media_files: {
        upload: 'media-files/upload',
        update: 'media-files/update',
        delete: (fileId: string) => `media-files/delete/${fileId}`,
        get: 'media-files/get',
    },
    services: {
        create: 'services/create',
        update: 'services/update',
        delete: (serviceId: string) => `services/delete/${serviceId}`,
        get: 'services/get',
    },
}
