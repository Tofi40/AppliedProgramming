import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AsyncPipe, CurrencyPipe, NgFor, NgIf } from '@angular/common';
import { RoomsService } from '../../../core/services/rooms.service';
import { RoomSummary, CreateBookingPayload } from '../../../core/models/room.model';
import { RequestsService } from '../../../core/services/requests.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  standalone: true,
  selector: 'app-room-detail',
  templateUrl: './room-detail.component.html',
  styleUrl: './room-detail.component.scss',
  imports: [RouterLink, CurrencyPipe, NgIf, NgFor, AsyncPipe]
})
export class RoomDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly roomsService = inject(RoomsService);
  private readonly requestsService = inject(RequestsService);
  private readonly authService = inject(AuthService);

  readonly room = signal<RoomSummary | null>(null);
  readonly loading = signal<boolean>(true);
  readonly requestNote = signal<string>('');
  readonly requestStatus = signal<'idle' | 'submitting' | 'success' | 'error'>('idle');

  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      const seekerId = this.authService.currentUser()?.id;
      this.roomsService.getRoom(id, seekerId ?? undefined).subscribe({
        next: (room) => {
          this.room.set(room);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
    }
  }

  canRequest(): boolean {
    const user = this.authService.currentUser();
    return !!user && user.role === 'Seeker';
  }

  submitRequest(): void {
    const room = this.room();
    const user = this.authService.currentUser();
    if (!room || !user) {
      return;
    }
    const payload: CreateBookingPayload = {
      roomId: room.id,
      note: this.requestNote()
    };
    this.requestStatus.set('submitting');
    this.requestsService.create(payload).subscribe({
      next: () => {
        this.requestStatus.set('success');
        this.requestNote.set('');
      },
      error: () => this.requestStatus.set('error')
    });
  }
}
