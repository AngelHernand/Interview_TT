import { Navigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import Spinner from '../UI/Spinner';

/**
 * Componente que redirige inteligentemente según el rol del usuario:
 *  - Admin → /dashboard
 *  - User  → /test
 *  - No autenticado → /login
 */
export default function SmartRedirect() {
  const { isAuthenticated, isAdmin, loading } = useAuth();

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <Spinner size="lg" />
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  if (isAdmin) {
    return <Navigate to="/dashboard" replace />;
  }

  return <Navigate to="/test" replace />;
}
