export type BookingRequestStatus = 'Pending' | 'Approved' | 'Declined' | 'Withdrawn';

export interface BookingRequest {
  id: string;
  roomId: string;
  seekerId: string;
  status: BookingRequestStatus;
  note?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface UpdateBookingRequestPayload {
  status: BookingRequestStatus;
  note?: string | null;
}
