import { describe, it, expect, vi, beforeEach } from 'vitest';
import authService from '../services/authService';

// Mock the api module
vi.mock('../services/api', () => {
  return {
    default: {
      post: vi.fn(),
      get: vi.fn(),
      interceptors: {
        request: { use: vi.fn() },
        response: { use: vi.fn() },
      },
    },
  };
});

import api from '../services/api';

describe('authService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    localStorage.clear();
  });

  describe('login', () => {
    it('debe almacenar token y usuario on success', async () => {
      const mockResponse = {
        data: {
          success: true,
          data: {
            token: 'jwt-token-123',
            refreshToken: 'refresh-token-456',
            usuario: { id: '1', nombre: 'Test', email: 'test@test.com', rol: 'User' },
            redirectUrl: '/test',
          },
          message: 'Login exitoso',
        },
      };
      api.post.mockResolvedValue(mockResponse);

      const result = await authService.login('test@test.com', 'Pass123!');

      expect(api.post).toHaveBeenCalledWith('/auth/login', {
        email: 'test@test.com',
        password: 'Pass123!',
      });
      expect(localStorage.getItem('token')).toBe('jwt-token-123');
      expect(localStorage.getItem('refreshToken')).toBe('refresh-token-456');
      expect(JSON.parse(localStorage.getItem('user'))).toEqual(
        expect.objectContaining({ email: 'test@test.com' })
      );
      expect(result.success).toBe(true);
    });

    it('no debe almacenar tokens on failure', async () => {
      const mockResponse = {
        data: {
          success: false,
          message: 'Credenciales inválidas',
        },
      };
      api.post.mockResolvedValue(mockResponse);

      const result = await authService.login('bad@test.com', 'wrong');

      expect(result.success).toBe(false);
      expect(localStorage.getItem('token')).toBeNull();
    });
  });

  describe('register', () => {
    it('debe almacenar token y usuario on success', async () => {
      const mockResponse = {
        data: {
          success: true,
          data: {
            token: 'new-token',
            refreshToken: 'new-refresh',
            usuario: { id: '2', nombre: 'Nuevo', email: 'nuevo@test.com', rol: 'User' },
          },
        },
      };
      api.post.mockResolvedValue(mockResponse);

      const result = await authService.register('Nuevo', 'nuevo@test.com', 'Pass123!', 'Pass123!');

      expect(api.post).toHaveBeenCalledWith('/auth/register', {
        nombre: 'Nuevo',
        email: 'nuevo@test.com',
        password: 'Pass123!',
        confirmPassword: 'Pass123!',
      });
      expect(localStorage.getItem('token')).toBe('new-token');
      expect(result.success).toBe(true);
    });

    it('no almacena tokens on failure', async () => {
      api.post.mockResolvedValue({
        data: { success: false, message: 'Email ya registrado' },
      });

      const result = await authService.register('X', 'dup@test.com', 'p', 'p');

      expect(result.success).toBe(false);
      expect(localStorage.getItem('token')).toBeNull();
    });
  });

  describe('logout', () => {
    it('debe limpiar localStorage', async () => {
      localStorage.setItem('token', 'tok');
      localStorage.setItem('refreshToken', 'ref');
      localStorage.setItem('user', '{}');

      api.post.mockResolvedValue({ data: { success: true } });

      await authService.logout();

      expect(localStorage.getItem('token')).toBeNull();
      expect(localStorage.getItem('refreshToken')).toBeNull();
      expect(localStorage.getItem('user')).toBeNull();
    });

    it('debe limpiar localStorage incluso si la API falla', async () => {
      localStorage.setItem('token', 'tok');
      localStorage.setItem('refreshToken', 'ref');
      localStorage.setItem('user', '{}');

      api.post.mockRejectedValue(new Error('Network error'));

      // logout uses try/finally, so the error may propagate
      // but localStorage should still be cleaned
      try {
        await authService.logout();
      } catch {
        // expected
      }

      expect(localStorage.getItem('token')).toBeNull();
      expect(localStorage.getItem('refreshToken')).toBeNull();
    });
  });

  describe('changePassword', () => {
    it('debe llamar al endpoint correcto', async () => {
      api.post.mockResolvedValue({ data: { success: true, message: 'OK' } });

      const result = await authService.changePassword('old', 'new', 'new');

      expect(api.post).toHaveBeenCalledWith('/auth/change-password', {
        currentPassword: 'old',
        newPassword: 'new',
        confirmNewPassword: 'new',
      });
      expect(result.success).toBe(true);
    });
  });

  describe('helpers', () => {
    it('getStoredUser retorna null si no hay usuario', () => {
      expect(authService.getStoredUser()).toBeNull();
    });

    it('getStoredUser retorna objeto parseado', () => {
      localStorage.setItem('user', JSON.stringify({ nombre: 'Test' }));
      expect(authService.getStoredUser()).toEqual({ nombre: 'Test' });
    });

    it('isAuthenticated retorna false sin token', () => {
      expect(authService.isAuthenticated()).toBe(false);
    });

    it('isAuthenticated retorna true con token', () => {
      localStorage.setItem('token', 'abc');
      expect(authService.isAuthenticated()).toBe(true);
    });

    it('getToken retorna el token almacenado', () => {
      localStorage.setItem('token', 'my-token');
      expect(authService.getToken()).toBe('my-token');
    });
  });
});
