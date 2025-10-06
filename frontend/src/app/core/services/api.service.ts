import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  get<T>(path: string, options?: Record<string, unknown>) {
    return this.http.get<T>(`${this.baseUrl}/${path}`, options);
  }

  post<T>(path: string, body: unknown, options?: Record<string, unknown>) {
    return this.http.post<T>(`${this.baseUrl}/${path}`, body, options);
  }

  patch<T>(path: string, body: unknown, options?: Record<string, unknown>) {
    return this.http.patch<T>(`${this.baseUrl}/${path}`, body, options);
  }

  delete<T>(path: string, options?: Record<string, unknown>) {
    return this.http.delete<T>(`${this.baseUrl}/${path}`, options);
  }
}
