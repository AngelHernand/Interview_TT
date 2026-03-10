import { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../services/authService';
import toast from 'react-hot-toast';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  // Cargar usuario al iniciar
  useEffect(() => {
    const storedUser = authService.getStoredUser();
    if (storedUser && authService.isAuthenticated()) {
      setUser(storedUser);
    }
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    try {
      const result = await authService.login(email, password);
      if (result.success) {
        setUser(result.data.usuario);
        toast.success(result.message || 'Login exitoso');
        // Redirigir según rol: Admin  adashboard, User a test
        const userRole = result.data.usuario?.rol;
        const redirectUrl = userRole === 'Admin' ? '/dashboard' : '/test';
        navigate(redirectUrl);
        return { success: true };
      }
      return { success: false, message: result.message };
    } catch (error) {
      const message = error.response?.data?.message || 'Error al iniciar sesión';
      return { success: false, message };
    }
  };

  const register = async (nombre, email, password, confirmPassword) => {
    try {
      const result = await authService.register(nombre, email, password, confirmPassword);
      if (result.success) {
        setUser(result.data.usuario);
        toast.success('Registro exitoso');
        // Usuarios nuevos son User por defecto a van al test
        const userRole = result.data.usuario?.rol;
        const redirectUrl = userRole === 'Admin' ? '/dashboard' : '/test';
        navigate(redirectUrl);
        return { success: true };
      }
      return { success: false, message: result.message, errors: result.errors };
    } catch (error) {
      const message = error.response?.data?.message || 'Error al registrarse';
      const errors = error.response?.data?.errors || [];
      return { success: false, message, errors };
    }
  };

  const logout = async () => {
    try {
      await authService.logout();
      setUser(null);
      toast.success('Sesión cerrada');
      navigate('/');
    } catch (error) {
      // limpiar estado local
      setUser(null);
      navigate('/');
    }
  };

  const updateUser = (updatedUser) => {
    setUser(updatedUser);
    localStorage.setItem('user', JSON.stringify(updatedUser));
  };

  const isAdmin = () => {
    return user?.rol === 'Admin';
  };

  const value = {
    user,
    loading,
    login,
    register,
    logout,
    updateUser,
    isAuthenticated: !!user,
    isAdmin: isAdmin(),
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth debe usarse dentro de AuthProvider');
  }
  return context;
}

export default AuthContext;
