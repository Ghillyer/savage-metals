import { useAuth } from './useAuth';

export function useIsAdmin() {
  const { currentUser } = useAuth();
  return currentUser?.role === 'Admin';
}