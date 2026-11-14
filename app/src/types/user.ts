export type UserRole = 'Purchaser' | 'Admin';

export interface CurrentUser {
  id: number;
  email: string;
  displayName: string;
  role: UserRole;
}