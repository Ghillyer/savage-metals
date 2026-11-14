import { Navigate, Outlet, useLocation } from 'react-router';
import { LoadingScreen } from '../components/LoadingScreen';
import { useAuth } from './useAuth';

export function RequireAuth() {
  const { currentUser, loading } = useAuth();
  const location = useLocation();

  if (loading) {
    return <LoadingScreen />;
  }

  if (!currentUser) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  return <Outlet />;
}