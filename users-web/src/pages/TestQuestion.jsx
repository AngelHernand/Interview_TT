import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import questionService from '../services/questionService';
import Spinner from '../components/UI/Spinner';
import toast from 'react-hot-toast';
import { ArrowRight, Send, HelpCircle } from 'lucide-react';

export default function TestQuestion() {
  const navigate = useNavigate();
  const [questions, setQuestions] = useState([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [answers, setAnswers] = useState({});
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    const fetchQuestions = async () => {
      try {
        const result = await questionService.getQuestions();
        if (result.success && result.data?.length > 0) {
          setQuestions(result.data);
        } else {
          toast.error('No se pudieron cargar las preguntas.');
          navigate('/test');
        }
      } catch {
        toast.error('Error al cargar las preguntas.');
        navigate('/test');
      } finally {
        setLoading(false);
      }
    };
    fetchQuestions();
  }, [navigate]);

  const currentQuestion = questions[currentIndex];
  const isLastQuestion = currentIndex === questions.length - 1;
  const selectedOption = answers[currentQuestion?.id] || null;
  const progress = questions.length > 0 ? ((currentIndex + 1) / questions.length) * 100 : 0;

  const handleSelectOption = (clave) => {
    setAnswers((prev) => ({ ...prev, [currentQuestion.id]: clave }));
  };

  const handleNext = async () => {
    if (!selectedOption) {
      toast.error('Selecciona una opcion antes de continuar.');
      return;
    }

    if (isLastQuestion) {
      setSubmitting(true);
      try {
        const payload = Object.entries(answers).map(([preguntaId, claveSeleccionada]) => ({
          preguntaId: parseInt(preguntaId),
          claveSeleccionada,
        }));
        const result = await questionService.evaluate(payload);
        if (result.success) {
          navigate('/test/stats', { state: { result: result.data } });
        } else {
          toast.error(result.message || 'Error al evaluar el test.');
        }
      } catch {
        toast.error('Error al enviar las respuestas.');
      } finally {
        setSubmitting(false);
      }
    } else {
      setCurrentIndex((prev) => prev + 1);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Spinner size="lg" />
      </div>
    );
  }

  if (!currentQuestion) return null;

  return (
    <div className="max-w-2xl mx-auto space-y-5">
      {/* Progress */}
      <div className="space-y-1.5">
        <div className="flex justify-between text-xs font-medium">
          <span className="text-neutral-600">Pregunta {currentIndex + 1} de {questions.length}</span>
          <span className="text-primary-600">{Math.round(progress)}%</span>
        </div>
        <div className="w-full bg-neutral-200 rounded-full h-2">
          <div
            className="h-2 rounded-full transition-all duration-500 ease-out"
            style={{
              width: `${progress}%`,
              background: 'linear-gradient(90deg, var(--color-primary-500), var(--color-accent-500))',
            }}
          />
        </div>
      </div>

      {/* Question */}
      <div className="card card-accent-top">
        <div className="flex items-start gap-3 mb-5">
          <div className="flex items-center justify-center w-8 h-8 rounded-lg bg-primary-100 shrink-0">
            <HelpCircle size={16} className="text-primary-600" />
          </div>
          <p className="text-sm font-semibold text-neutral-900 pt-1">
            {currentQuestion.texto}
          </p>
        </div>

        <div className="space-y-2">
          {currentQuestion.opciones?.map((opcion) => {
            const isSelected = selectedOption === opcion.clave;
            return (
              <button
                key={opcion.clave}
                onClick={() => handleSelectOption(opcion.clave)}
                className={`w-full text-left px-4 py-3 rounded-lg border text-sm transition-all duration-200 cursor-pointer ${
                  isSelected
                    ? 'border-primary-500 bg-primary-50 text-primary-800 shadow-sm'
                    : 'border-neutral-200 hover:border-primary-300 hover:bg-primary-50/30 text-neutral-700'
                }`}
              >
                <span
                  className={`inline-flex items-center justify-center w-7 h-7 rounded-lg text-xs font-bold mr-3 transition-all duration-200 ${
                    isSelected
                      ? 'bg-primary-500 text-white shadow-sm'
                      : 'bg-neutral-100 text-neutral-600'
                  }`}
                >
                  {opcion.clave}
                </span>
                {opcion.texto}
              </button>
            );
          })}
        </div>
      </div>

      {/* Action */}
      <div className="flex justify-end">
        <button
          onClick={handleNext}
          disabled={!selectedOption || submitting}
          className="btn btn-primary disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {submitting ? (
            <>
              <Spinner size="sm" /> Enviando...
            </>
          ) : isLastQuestion ? (
            <>
              Finalizar test
              <Send size={15} />
            </>
          ) : (
            <>
              Siguiente
              <ArrowRight size={15} />
            </>
          )}
        </button>
      </div>
    </div>
  );
}
