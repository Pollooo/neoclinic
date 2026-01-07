import { Injectable, signal, inject } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { LocalStorageService } from './local-storage.service';

export type Language = 'uz' | 'ru';

export interface Translations {
  [key: string]: string | Translations;
}

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private readonly localStorageService = inject(LocalStorageService);
  private readonly router = inject(Router);
  public currentLanguage = signal<Language>('uz');

  constructor() {
    // Initialize language from localStorage first
    const savedLanguage = this.localStorageService.getLanguage();
    if (savedLanguage) {
      this.currentLanguage.set(savedLanguage as Language);
    }
    
    // Then initialize from route or localStorage
    this.initializeLanguageFromRoute();
    
    // Listen to route changes
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.initializeLanguageFromRoute();
    });
  }

  private initializeLanguageFromRoute(): void {
    const url = this.router.url;
    const langMatch = url.match(/^\/(uz|ru)/);
    
    if (langMatch) {
      const routeLang = langMatch[1] as Language;
      if (this.currentLanguage() !== routeLang) {
        this.currentLanguage.set(routeLang);
        this.localStorageService.setLanguage(routeLang);
      }
    }
  }

  private translations: Record<Language, Translations> = {
    uz: {
      // Common
      common: {
        welcome: 'Xush kelibsiz',
        home: 'Bosh sahifa',
        services: 'Xizmatlar',
        doctors: 'Shifokorlar',
        appointment: 'Qabul',
        contact: 'Aloqa',
        about: 'Biz haqimizda',
        gallery: 'Galereya',
        login: 'Kirish',
        logout: 'Chiqish',
        submit: 'Yuborish',
        cancel: 'Bekor qilish',
        save: 'Saqlash',
        delete: 'O\'chirish',
        edit: 'Tahrirlash',
        create: 'Yaratish',
        update: 'Yangilash',
        close: 'Yopish',
        search: 'Qidirish',
        loading: 'Yuklanmoqda...',
        noData: 'Ma\'lumot topilmadi',
        language: 'Til',
      },
      // Appointment
      appointment: {
        title: 'Qabulga yozilish',
        fullName: 'Ism-familiya',
        phoneNumber: 'Telefon raqami',
        email: 'Email',
        description: 'Qo\'shimcha ma\'lumot',
        selectDate: 'Sanani tanlang',
        selectDoctor: 'Shifokorni tanlang',
        selectService: 'Xizmatni tanlang',
        success: 'Qabulga muvaffaqiyatli yozildingiz!',
        error: 'Xatolik yuz berdi. Qayta urinib ko\'ring.',
      },
      // Contact
      contact: {
        title: 'Biz bilan bog\'laning',
        name: 'Kompaniya nomi',
        email: 'Email',
        phone: 'Telefon',
        message: 'Xabar',
        send: 'Yuborish',
        success: 'Xabaringiz yuborildi!',
        error: 'Xatolik yuz berdi.',
        address: 'Manzil',
        workingHours: 'Ish vaqti',
      },
      // Admin
      admin: {
        dashboard: 'Boshqaruv paneli',
        appointments: 'Qabullar',
        messages: 'Xabarlar',
        doctorsManagement: 'Shifokorlar',
        servicesManagement: 'Xizmatlar',
        mediaFiles: 'Media fayllar',
        username: 'Foydalanuvchi nomi',
        password: 'Parol',
        loginTitle: 'Tizimga kirish',
        loginButton: 'Kirish',
        invalidCredentials: 'Login yoki parol noto\'g\'ri',
      },
      // Services
      services: {
        title: 'Bizning xizmatlarimiz',
        description: 'Tavsif',
        price: 'Narx',
        duration: 'Davomiyligi',
      },
      // Doctors
      doctors: {
        title: 'Bizning shifokorlar',
        specialization: 'Mutaxassisligi',
        experience: 'Tajriba',
        education: 'Ta\'lim',
      },
      // Notifications
      notifications: {
        success: 'Muvaffaqiyatli!',
        error: 'Xatolik!',
        warning: 'Diqqat!',
        info: 'Ma\'lumot',
      }
    },
    ru: {
      // Common
      common: {
        welcome: 'Добро пожаловать',
        home: 'Главная',
        services: 'Услуги',
        doctors: 'Врачи',
        appointment: 'Запись',
        contact: 'Контакты',
        about: 'О нас',
        gallery: 'Галерея',
        login: 'Войти',
        logout: 'Выйти',
        submit: 'Отправить',
        cancel: 'Отмена',
        save: 'Сохранить',
        delete: 'Удалить',
        edit: 'Редактировать',
        create: 'Создать',
        update: 'Обновить',
        close: 'Закрыть',
        search: 'Поиск',
        loading: 'Загрузка...',
        noData: 'Данные не найдены',
        language: 'Язык',
      },
      // Appointment
      appointment: {
        title: 'Запись на прием',
        fullName: 'ФИО',
        phoneNumber: 'Номер телефона',
        email: 'Email',
        description: 'Дополнительная информация',
        selectDate: 'Выберите дату',
        selectDoctor: 'Выберите врача',
        selectService: 'Выберите услугу',
        success: 'Вы успешно записались на прием!',
        error: 'Произошла ошибка. Попробуйте еще раз.',
      },
      // Contact
      contact: {
        title: 'Свяжитесь с нами',
        name: 'Название компании',
        email: 'Email',
        phone: 'Телефон',
        message: 'Сообщение',
        send: 'Отправить',
        success: 'Ваше сообщение отправлено!',
        error: 'Произошла ошибка.',
        address: 'Адрес',
        workingHours: 'Часы работы',
      },
      // Admin
      admin: {
        dashboard: 'Панель управления',
        appointments: 'Записи',
        messages: 'Сообщения',
        doctorsManagement: 'Врачи',
        servicesManagement: 'Услуги',
        mediaFiles: 'Медиа файлы',
        username: 'Имя пользователя',
        password: 'Пароль',
        loginTitle: 'Вход в систему',
        loginButton: 'Войти',
        invalidCredentials: 'Неверный логин или пароль',
      },
      // Services
      services: {
        title: 'Наши услуги',
        description: 'Описание',
        price: 'Цена',
        duration: 'Продолжительность',
      },
      // Doctors
      doctors: {
        title: 'Наши врачи',
        specialization: 'Специализация',
        experience: 'Опыт',
        education: 'Образование',
      },
      // Notifications
      notifications: {
        success: 'Успешно!',
        error: 'Ошибка!',
        warning: 'Внимание!',
        info: 'Информация',
      }
    }
  };

  public setLanguage(language: Language): void {
    this.currentLanguage.set(language);
    this.localStorageService.setLanguage(language);
    
    // Navigate to the new language route
    const currentUrl = this.router.url;
    const urlWithoutLang = currentUrl.replace(/^\/(uz|ru)/, '');
    const newUrl = `/${language}${urlWithoutLang || ''}`;
    this.router.navigateByUrl(newUrl);
  }

  public translate(key: string): string {
    const keys = key.split('.');
    let value: any = this.translations[this.currentLanguage()];

    for (const k of keys) {
      if (value && typeof value === 'object' && k in value) {
        value = value[k];
      } else {
        return key; // Return key if translation not found
      }
    }

    return typeof value === 'string' ? value : key;
  }

  public t = this.translate.bind(this);
}

