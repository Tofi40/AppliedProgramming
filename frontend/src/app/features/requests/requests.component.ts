import { Component, inject, OnInit, signal } from '@angular/core';
import { AsyncPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { RequestsService } from '../../core/services/requests.service';
import { BookingRequest, BookingRequestStatus } from '../../core/models/request.model';
import { AuthService } from '../../core/services/auth.service';

@Component({
  standalone: true,
  selector: 'app-requests',
  templateUrl: './requests.component.html',
  styleUrl: './requests.component.scss',
  imports: [NgIf, NgFor, DatePipe, AsyncPipe]
})
export class RequestsComponent implements OnInit {
  private readonly requestsService = inject(RequestsService);
  private readonly authService = inject(AuthService);

  readonly requests = signal<BookingRequest[]>([]);
  readonly loading = signal<boolean>(true);
  readonly statuses: BookingRequestStatus[] = ['Pending', 'Approved', 'Declined', 'Withdrawn'];
  readonly isOwner = signal<boolean>(false);

  ngOnInit(): void {
    const user = this.authService.currentUser();
    this.isOwner.set(user?.role === 'Owner');
    this.refresh();
  }

  refresh(): void {
    this.loading.set(true);
    this.requestsService.list().subscribe({
      next: (requests) => {
        this.requests.set(requests);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  updateStatus(request: BookingRequest, status: string): void {
    if (!this.isOwner()) {
      return;
    }
    const nextStatus = status as BookingRequestStatus;
    this.requestsService.update(request.id, { status: nextStatus, note: request.note }).subscribe({
      next: (updated) => {
        this.requests.update((items) => items.map((item) => (item.id === updated.id ? updated : item)));
      }
    });
  }
}
