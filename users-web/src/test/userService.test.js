import { describe, it, expect, vi, beforeEach } from 'vitest';
import userService from '../services/userService';

vi.mock('../services/api', () => {
  return {
    default: {
      get: vi.fn(),
      post: vi.fn(),
      put: vi.fn(),
      delete: vi.fn(),
      interceptors: {
        request: { use: vi.fn() },
        response: { use: vi.fn() },
      },
    },
  };
});

import api from '../services/api';

describe('userService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('getAll', () => {
    it('debe enviar page y pageSize como params', async () => {
      api.get.mockResolvedValue({
        data: {
          success: true,
          data: { items: [], totalCount: 0, page: 1, pageSize: 10 },
        },
      });

      const result = await userService.getAll(2, 5);

      expect(api.get).toHaveBeenCalledWith('/users', { params: { page: 2, pageSize: 5 } });
      expect(result.success).toBe(true);
    });

    it('debe usar defaults page=1, pageSize=10', async () => {
      api.get.mockResolvedValue({
        data: { success: true, data: { items: [] } },
      });

      await userService.getAll();

      expect(api.get).toHaveBeenCalledWith('/users', { params: { page: 1, pageSize: 10 } });
    });
  });

  describe('getById', () => {
    it('debe llamar a GET /users/:id', async () => {
      const mockUser = { id: 'abc-123', nombre: 'Test', email: 'test@test.com' };
      api.get.mockResolvedValue({
        data: { success: true, data: mockUser },
      });

      const result = await userService.getById('abc-123');

      expect(api.get).toHaveBeenCalledWith('/users/abc-123');
      expect(result.data.nombre).toBe('Test');
    });
  });

  describe('getCurrentUser', () => {
    it('debe llamar a GET /users/me', async () => {
      api.get.mockResolvedValue({
        data: { success: true, data: { nombre: 'Me' } },
      });

      const result = await userService.getCurrentUser();

      expect(api.get).toHaveBeenCalledWith('/users/me');
      expect(result.data.nombre).toBe('Me');
    });
  });

  describe('create', () => {
    it('debe enviar POST /users con datos del usuario', async () => {
      const userData = { nombre: 'Nuevo', email: 'n@t.com', password: 'P1!', rolId: 2 };
      api.post.mockResolvedValue({
        data: { success: true, data: { ...userData, id: 'new-id' }, message: 'Creado' },
      });

      const result = await userService.create(userData);

      expect(api.post).toHaveBeenCalledWith('/users', userData);
      expect(result.success).toBe(true);
      expect(result.data.id).toBe('new-id');
    });
  });

  describe('update', () => {
    it('debe enviar PUT /users/:id con datos parciales', async () => {
      api.put.mockResolvedValue({
        data: { success: true, message: 'Actualizado' },
      });

      const result = await userService.update('id-1', { nombre: 'Editado' });

      expect(api.put).toHaveBeenCalledWith('/users/id-1', { nombre: 'Editado' });
      expect(result.success).toBe(true);
    });
  });

  describe('delete', () => {
    it('debe enviar DELETE /users/:id', async () => {
      api.delete.mockResolvedValue({
        data: { success: true, message: 'Eliminado' },
      });

      const result = await userService.delete('id-2');

      expect(api.delete).toHaveBeenCalledWith('/users/id-2');
      expect(result.success).toBe(true);
    });
  });

  describe('toggleBlock', () => {
    it('debe enviar PUT /users/:id/block', async () => {
      api.put.mockResolvedValue({
        data: { success: true, message: 'Bloqueado' },
      });

      const result = await userService.toggleBlock('id-3');

      expect(api.put).toHaveBeenCalledWith('/users/id-3/block');
      expect(result.success).toBe(true);
    });
  });
});
