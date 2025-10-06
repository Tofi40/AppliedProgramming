export interface UserProfile {
  id: string;
  email: string;
  displayName: string;
  role: 'Owner' | 'Seeker';
  avatarUrl?: string | null;
  city: string;
  country: string;
  onboardingComplete: boolean;
  bio?: string | null;
  interests?: string[];
  habits?: string[];
  personality?: string[];
}

export interface UpdateUserPayload {
  displayName?: string;
  bio?: string;
  city?: string;
  country?: string;
  interests?: string[];
  habits?: string[];
  personality?: string[];
}
