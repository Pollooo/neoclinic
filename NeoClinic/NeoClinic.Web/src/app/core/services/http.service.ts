import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { catchError, Observable, throwError } from "rxjs";
import { HttpErrorResponse } from "@angular/common/http";

@Injectable({
    providedIn: 'root',
})
export class HttpService {
    private readonly httpClient = inject(HttpClient);
    private baseUrl = environment.apiBaseUrl;

    public get<T>(endpoint: string, options = {}): Observable<T> {
        return this.httpClient.get<T>(`${this.baseUrl}/${endpoint}`, options).
        pipe(catchError(this.handleError));
    }

    public post<T>(endpoint: string, body: any, options = {}): Observable<T> {
        return this.httpClient.post<T>(`${this.baseUrl}/${endpoint}`, body, options)
          .pipe(catchError(this.handleError));
      }
    
      public put<T>(endpoint: string, body: any, options = {}): Observable<T> {
        return this.httpClient.put<T>(`${this.baseUrl}/${endpoint}`, body, options)
          .pipe(catchError(this.handleError));
      }
    
      public delete<T>(endpoint: string, options = {}): Observable<T> {
        return this.httpClient.delete<T>(`${this.baseUrl}/${endpoint}`, options)
          .pipe(catchError(this.handleError));
      }

    private handleError(error: HttpErrorResponse) {
        let message = 'An unexpected error occurred. Please try again later.';
        
        if (error.error && typeof error.error === 'object' && 'message' in error.error) {
          message = error.error.message;
        }
        else if (error.error && typeof error.error === 'object' && 'detail' in error.error) {
          message = error.error.detail;
        }
        else if (error.error && typeof error.error === 'object' && 'title' in error.error) {
          message = error.error.title;
        }
        else if (typeof error.error === 'string' && error.error.trim()) {
          message = error.error;
        }
        else {
          if (error.status === 0)
            message = 'Unable to connect to the server. Please check your internet connection or try again later.';
          else if (error.status === 400)
            message = 'The request was invalid or incomplete. Please review your input and try again.';
          else if (error.status === 401)
            message = 'You are not authorized to perform this action. Please log in and try again.';
          else if (error.status === 403)
            message = 'Access denied. You do not have permission to view this resource.';
          else if (error.status === 404)
            message = 'The requested resource could not be found. It might have been removed or is temporarily unavailable.';
          else if (error.status >= 500)
            message = 'The server encountered an error while processing your request. Please try again later.';
        }
    
        return throwError(() => ({
          message: message,
          error: error.error,
          status: error.status,
          statusText: error.statusText
        }));
      }
}