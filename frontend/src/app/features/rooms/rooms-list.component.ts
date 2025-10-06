import { Component, computed, inject, signal } from '@angular/core';
import { AsyncPipe, CurrencyPipe, NgFor, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RoomsService } from '../../core/services/rooms.service';
import { RoomSummary } from '../../core/models/room.model';
import { AuthService } from '../../core/services/auth.service';

@Component({
  standalone: true,
  selector: 'app-rooms-list',
  templateUrl: './rooms-list.component.html',
  styleUrl: './rooms-list.component.scss',
  imports: [NgIf, NgFor, CurrencyPipe, RouterLink, AsyncPipe]
})
export class RoomsListComponent {
  private readonly roomsService = inject(RoomsService);
  private readonly authService = inject(AuthService);

  readonly rooms = signal<RoomSummary[]>([]);
  readonly loading = signal<boolean>(false);
  readonly cityFilter = signal<string>('');
  readonly onlyPublished = signal<boolean>(true);

  readonly filteredRooms = computed(() => {
    return this.rooms()
      .filter((room) => !this.onlyPublished() || room.isPublished)
      .filter((room) =>
        !this.cityFilter() ? true : `${room.city}, ${room.country}`.toLowerCase().includes(this.cityFilter().toLowerCase())
      );
  });

  constructor() {
    this.loadRooms();
  }

  loadRooms(): void {
    this.loading.set(true);
    const seekerId = this.authService.currentUser()?.id;
    this.roomsService.listRooms(seekerId ?? undefined).subscribe({
      next: (rooms) => {
        this.rooms.set(rooms);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
