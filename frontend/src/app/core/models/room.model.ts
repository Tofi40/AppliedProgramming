export interface RoomSummary {
  id: string;
  title: string;
  description: string;
  city: string;
  country: string;
  pricePerMonthCents: number;
  billsIncluded: boolean;
  isPublished: boolean;
  compatibilityScore?: number | null;
  rationale?: string[] | null;
}

export interface CreateBookingPayload {
  roomId: string;
  note?: string | null;
}
