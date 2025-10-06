import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { BookingRequest, UpdateBookingRequestPayload } from '../models/request.model';
import { CreateBookingPayload } from '../models/room.model';

@Injectable({ providedIn: 'root' })
export class RequestsService {
  private readonly api = inject(ApiService);

  list(): Observable<BookingRequest[]> {
    return this.api.get<BookingRequest[]>('requests');
  }

  create(payload: CreateBookingPayload): Observable<BookingRequest> {
    return this.api.post<BookingRequest>('requests', payload);
  }

  update(id: string, payload: UpdateBookingRequestPayload): Observable<BookingRequest> {
    return this.api.patch<BookingRequest>(`requests/${id}`, payload);
  }
}
