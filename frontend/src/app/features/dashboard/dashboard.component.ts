import { Component, inject, OnInit, signal } from '@angular/core';
import { AsyncPipe, CurrencyPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { RoomsService } from '../../core/services/rooms.service';
import { RequestsService } from '../../core/services/requests.service';
import { RoomSummary } from '../../core/models/room.model';
import { BookingRequest } from '../../core/models/request.model';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  imports: [NgIf, NgFor, CurrencyPipe, DatePipe, RouterLink, AsyncPipe]
})
export class DashboardComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly roomsService = inject(RoomsService);
  private readonly requestsService = inject(RequestsService);

  readonly rooms = signal<RoomSummary[]>([]);
  readonly requests = signal<BookingRequest[]>([]);
  readonly loadingRooms = signal<boolean>(true);
  readonly loadingRequests = signal<boolean>(true);

  readonly user$ = this.authService.user$;

  ngOnInit(): void {
    if (!this.authService.currentUser()) {
      return;
    }
    this.fetchRooms();
    this.fetchRequests();
  }

  refreshRooms(): void {
    this.fetchRooms();
  }

  refreshRequests(): void {
    this.fetchRequests();
  }

  isOwner(): boolean {
    return this.authService.currentUser()?.role === 'Owner';
  }

  private fetchRooms(): void {
    const user = this.authService.currentUser();
    if (!user) {
      return;
    }
    this.loadingRooms.set(true);
    this.roomsService.listRooms(user.role === 'Seeker' ? user.id : undefined).subscribe({
      next: (rooms) => {
        this.rooms.set(rooms);
        this.loadingRooms.set(false);
      },
      error: () => this.loadingRooms.set(false)
    });
  }

  private fetchRequests(): void {
    this.loadingRequests.set(true);
    this.requestsService.list().subscribe({
      next: (requests) => {
        this.requests.set(requests);
        this.loadingRequests.set(false);
      },
      error: () => this.loadingRequests.set(false)
    });
  }
}
