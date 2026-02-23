import api from './api';

const questionService = {
  /**
   * Obtiene las 15 preguntas del test.
   * @returns {Promise<{success: boolean, data: Array, message: string}>}
   */
  getQuestions: async () => {
    try {
      const response = await api.get('/questions');
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  /**
   * Envía las respuestas y obtiene el resultado del test.
    @param {Array<{preguntaId: number, claveSeleccionada: string}>} answers
    @returns {Promise<{success: boolean, data: object, message: string}>}
   */
  evaluate: async (answers) => {
    try {
      const response = await api.post('/questions/evaluate', answers);
      return response.data;
    } catch (error) {
      throw error;
    }
  },
};

export default questionService;
