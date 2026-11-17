export type UserRole = 'Purchaser' | 'Admin';

export interface CurrentUser {
  id: number;
  email: string;
  displayName: string;
  role: UserRole;
}

export interface AppUser {
  id: number;
  email: string;
  displayName: string;
  role: UserRole;
  isActive: boolean;
}

export interface CreateUserRequest {
  email: string;
  displayName: string;
  role: UserRole;
}

export interface UpdateUserRequest {
  displayName: string;
  role: UserRole;
  isActive: boolean;
}