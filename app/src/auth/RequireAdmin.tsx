import { Navigate, Outlet } from 'react-router';
import { useIsAdmin } from './useIsAdmin';

export function RequireAdmin() {
  const isAdmin = useIsAdmin();

  if (!isAdmin) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}