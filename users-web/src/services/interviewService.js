import api from './api';

const interviewService = {
  // Iniciar una nueva entrevista simulada
  async startInterview(data) {
    const response = await api.post('/interviews', data);
    return response.data;
  },

  // Enviar mensaje del candidato y recibir respuesta del entrevistador
  async sendMessage(sessionId, mensaje) {
    const response = await api.post(`/interviews/${sessionId}/messages`, { mensaje });
    return response.data;
  },

  // Finalizar entrevista y obtener evaluacion
  async endInterview(sessionId) {
    const response = await api.post(`/interviews/${sessionId}/end`);
    return response.data;
  },

  // Obtener sesion con historial de mensajes
  async getSession(sessionId) {
    const response = await api.get(`/interviews/${sessionId}`);
    return response.data;
  },

  // Listar sesiones del usuario (paginado)
  async getUserSessions(page = 1, pageSize = 10) {
    const response = await api.get(`/interviews?page=${page}&pageSize=${pageSize}`);
    return response.data;
  },
};

export default interviewService;
