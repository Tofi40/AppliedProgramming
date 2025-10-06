import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { UpdateUserPayload, UserProfile } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private readonly api = inject(ApiService);

  me(): Observable<UserProfile> {
    return this.api.get<UserProfile>('users/me');
  }

  updateMe(payload: UpdateUserPayload): Observable<UserProfile> {
    return this.api.patch<UserProfile>('users/me', payload);
  }
}
