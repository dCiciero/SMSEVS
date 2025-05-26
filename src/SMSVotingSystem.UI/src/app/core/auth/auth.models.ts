export interface AuthModels {}

export interface User {
  id: string;
  userName: string;
  email: string;
  roles: string[];
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiration: string;
  user: User;
}

export interface LoginDto {
  userName: string;
  password: string;
}

export interface RegisterDto {
  userName: string;
  email: string;
  password: string;
}
