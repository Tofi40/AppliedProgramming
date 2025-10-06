export interface MatchUserSummary {
  id: string;
  displayName: string;
  bio?: string | null;
  avatarUrl?: string | null;
  age?: number | null;
  gender?: string | null;
  pronouns?: string | null;
  occupation?: string | null;
  city?: string | null;
  country?: string | null;
  interests?: string[] | null;
  habits?: string[] | null;
  personality?: string[] | null;
}

export interface MatchRoomSummary {
  id: string;
  title: string;
  description: string;
  photos?: string[] | null;
  city: string;
  country: string;
  pricePerMonthCents: number;
  billsIncluded: boolean;
  minTermMonths?: number;
  amenities?: string[] | null;
  houseRules?: string[] | null;
  availableFrom?: string | null;
}

export interface Match {
  id: string;
  seekerId: string;
  ownerId: string;
  roomId: string;
  compatibilityScore: number;
  rationale?: string | null;
  createdAt: string;
  seeker: MatchUserSummary;
  owner: MatchUserSummary;
  room: MatchRoomSummary;
}
