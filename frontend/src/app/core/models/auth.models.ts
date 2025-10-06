export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  displayName: string;
  role: 'Owner' | 'Seeker';
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface SessionUser {
  id: string;
  email: string;
  displayName: string;
  role: 'Owner' | 'Seeker';
  expiresAt: Date;
}
