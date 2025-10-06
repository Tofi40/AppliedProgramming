import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Match } from '../models/match.model';

@Injectable({ providedIn: 'root' })
export class MatchesService {
  private readonly api = inject(ApiService);

  forSeeker(seekerId: string): Observable<Match[]> {
    return this.api.get<Match[]>(`matches/seeker/${seekerId}`);
  }

  forOwner(ownerId: string): Observable<Match[]> {
    return this.api.get<Match[]>(`matches/owner/${ownerId}`);
  }
}
