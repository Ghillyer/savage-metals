import { createContext } from 'react';
import type { User } from 'firebase/auth';
import type { CurrentUser } from '../types/user';

export interface AuthContextValue {
  firebaseUser: User | null;
  currentUser: CurrentUser | null;
  loading: boolean;
  signIn: (email: string, password: string) => Promise<void>;
  signOut: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | null>(null);