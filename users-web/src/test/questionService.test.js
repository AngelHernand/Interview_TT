import { describe, it, expect, vi, beforeEach } from 'vitest';
import questionService from '../services/questionService';

vi.mock('../services/api', () => {
  return {
    default: {
      get: vi.fn(),
      post: vi.fn(),
      interceptors: {
        request: { use: vi.fn() },
        response: { use: vi.fn() },
      },
    },
  };
});

import api from '../services/api';

describe('questionService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('getQuestions', () => {
    it('debe llamar a GET /questions', async () => {
      const mockQuestions = [
        { id: 1, texto: 'Pregunta 1', opciones: [{ clave: 'A', texto: 'Op A' }] },
        { id: 2, texto: 'Pregunta 2', opciones: [{ clave: 'B', texto: 'Op B' }] },
      ];
      api.get.mockResolvedValue({
        data: { success: true, data: mockQuestions, message: 'OK' },
      });

      const result = await questionService.getQuestions();

      expect(api.get).toHaveBeenCalledWith('/questions');
      expect(result.success).toBe(true);
      expect(result.data).toHaveLength(2);
      expect(result.data[0].texto).toBe('Pregunta 1');
    });

    it('debe lanzar error si la API falla', async () => {
      api.get.mockRejectedValue(new Error('Network error'));

      await expect(questionService.getQuestions()).rejects.toThrow('Network error');
    });
  });

  describe('evaluate', () => {
    it('debe enviar respuestas al endpoint correcto', async () => {
      const answers = [
        { preguntaId: 1, claveSeleccionada: 'B' },
        { preguntaId: 2, claveSeleccionada: 'C' },
      ];
      const mockResult = {
        totalPreguntas: 15,
        correctas: 2,
        incorrectas: 13,
        porcentajeAcierto: 13.33,
        detalle: [],
      };
      api.post.mockResolvedValue({
        data: { success: true, data: mockResult, message: 'Evaluado' },
      });

      const result = await questionService.evaluate(answers);

      expect(api.post).toHaveBeenCalledWith('/questions/evaluate', answers);
      expect(result.success).toBe(true);
      expect(result.data.correctas).toBe(2);
      expect(result.data.totalPreguntas).toBe(15);
    });

    it('debe lanzar error si la evaluación falla', async () => {
      api.post.mockRejectedValue(new Error('Server error'));

      await expect(questionService.evaluate([])).rejects.toThrow('Server error');
    });
  });
});
