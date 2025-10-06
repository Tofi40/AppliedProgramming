import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { RoomSummary } from '../models/room.model';

@Injectable({ providedIn: 'root' })
export class RoomsService {
  private readonly api = inject(ApiService);

  listRooms(seekerId?: string): Observable<RoomSummary[]> {
    const params = seekerId ? { params: { seekerId } } : undefined;
    return this.api.get<RoomSummary[]>('rooms', params);
  }

  getRoom(id: string, seekerId?: string): Observable<RoomSummary> {
    const params = seekerId ? { params: { seekerId } } : undefined;
    return this.api.get<RoomSummary>(`rooms/${id}`, params);
  }
}
