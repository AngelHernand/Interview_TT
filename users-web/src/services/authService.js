import api from './api';

const authService = {
  
   // Login con email y password
   
  async login(email, password) {
    const response = await api.post('/auth/login', { email, password });
    if (response.data.success) {
      const { token, refreshToken, usuario, redirectUrl } = response.data.data;
      localStorage.setItem('token', token);
      localStorage.setItem('refreshToken', refreshToken);
      localStorage.setItem('user', JSON.stringify(usuario));
      return { ...response.data, redirectUrl };
    }
    return response.data;
  },

   // Registrar nuevo usuario
   
  async register(nombre, email, password, confirmPassword) {
    const response = await api.post('/auth/register', {
      nombre,
      email,
      password,
      confirmPassword,
    });
    if (response.data.success) {
      const { token, refreshToken, usuario } = response.data.data;
      localStorage.setItem('token', token);
      localStorage.setItem('refreshToken', refreshToken);
      localStorage.setItem('user', JSON.stringify(usuario));
    }
    return response.data;
  },

   // Cerrar sesión
  async logout() {
    const refreshToken = localStorage.getItem('refreshToken');
    try {
      if (refreshToken) {
        await api.post('/auth/logout', { refreshToken });
      }
    } finally {
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('user');
    }
  },

   // Renovar token
  async refreshToken() {
    const refreshToken = localStorage.getItem('refreshToken');
    if (!refreshToken) throw new Error('No refresh token');

    const response = await api.post('/auth/refresh', { refreshToken });
    if (response.data.success) {
      const { token, refreshToken: newRefreshToken } = response.data.data;
      localStorage.setItem('token', token);
      localStorage.setItem('refreshToken', newRefreshToken);
    }
    return response.data;
  },

  
   // Cambiar contraseña
   
  async changePassword(currentPassword, newPassword, confirmNewPassword) {
    const response = await api.post('/auth/change-password', {
      currentPassword,
      newPassword,
      confirmNewPassword,
    });
    return response.data;
  },

  
   // Obtener usuario almacenado localmente
   
  getStoredUser() {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  },

  // Verificar si hay sesión activa
   
  isAuthenticated() {
    return !!localStorage.getItem('token');
  },

  
  // Obtener token actual
   
  getToken() {
    return localStorage.getItem('token');
  },
};

export default authService;
