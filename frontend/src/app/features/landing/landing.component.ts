import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AsyncPipe, CurrencyPipe, NgFor, NgIf } from '@angular/common';
import { RoomsService } from '../../core/services/rooms.service';
import { RoomSummary } from '../../core/models/room.model';
import { AuthService } from '../../core/services/auth.service';

@Component({
  standalone: true,
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss',
  imports: [RouterLink, NgFor, CurrencyPipe, NgIf, AsyncPipe]
})
export class LandingComponent implements OnInit {
  private readonly roomsService = inject(RoomsService);
  private readonly authService = inject(AuthService);

  featuredRooms: RoomSummary[] = [];
  readonly isAuthenticated$ = this.authService.isAuthenticated$;

  ngOnInit(): void {
    const seekerId = this.authService.currentUser()?.id;
    this.roomsService.listRooms(seekerId ?? undefined).subscribe((rooms) => {
      this.featuredRooms = rooms.slice(0, 3);
    });
  }
}
