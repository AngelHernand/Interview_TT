import { useAuth } from '../context/AuthContext';


 // Hook personalizado para acceder al contexto de autenticación.
 // Re-exporta useAuth del contexto para uso conveniente.
export { useAuth };


// Hook para verificar permisos de rol
 
export function useRole() {
  const { user, isAdmin } = useAuth();
  
  return {
    role: user?.rol || null,
    isAdmin,
    isUser: user?.rol === 'User',
    hasRole: (role) => user?.rol === role,
  };
}
