import api from './api';

const userService = {
  
   // Listar usuarios con paginación
   
  async getAll(page = 1, pageSize = 10) {
    const response = await api.get('/users', {
      params: { page, pageSize },
    });
    return response.data;
  },

  
   // Obtener usuario por ID
   
  async getById(id) {
    const response = await api.get(`/users/${id}`);
    return response.data;
  },

  
  // Obtener usuario actual 
  
  async getCurrentUser() {
    const response = await api.get('/users/me');
    return response.data;
  },

   // Crear nuevo usuario (solo Admin)
   
  async create(userData) {
    const response = await api.post('/users', userData);
    return response.data;
  },

  
  // Actualizar usuario
   
  async update(id, userData) {
    const response = await api.put(`/users/${id}`, userData);
    return response.data;
  },

  
  // Eliminar usuario (soft delete)
   
  async delete(id) {
    const response = await api.delete(`/users/${id}`);
    return response.data;
  },

  
  // Bloquear/desbloquear usuario (solo Admin)
   
  async toggleBlock(id) {
    const response = await api.put(`/users/${id}/block`);
    return response.data;
  },
};

export default userService;
