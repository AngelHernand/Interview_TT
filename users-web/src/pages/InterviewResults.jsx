import { useState, useEffect } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import interviewService from '../services/interviewService';
import Spinner from '../components/UI/Spinner';
import {
  Trophy,
  ArrowLeft,
  RotateCcw,
  Star,
  TrendingUp,
  TrendingDown,
  CheckCircle2,
  AlertTriangle,
  XCircle,
  Award,
} from 'lucide-react';

export default function InterviewResults() {
  const { sessionId } = useParams();
  const location = useLocation();
  const navigate = useNavigate();

  const [evaluation, setEvaluation] = useState(location.state?.evaluation || null);
  const [session, setSession] = useState(null);
  const [loading, setLoading] = useState(!location.state?.evaluation);

  useEffect(() => {
    if (evaluation) return;

    const load = async () => {
      try {
        const result = await interviewService.getSession(sessionId);
        if (result.success && result.data.estado === 'Finalizada') {
          setSession(result.data);
          // La evaluacion viene en la sesion si ya fue finalizada
          // Intentar finalizar si no esta finalizada
          const endResult = await interviewService.endInterview(sessionId);
          if (endResult.success) {
            setEvaluation(endResult.data);
          }
        } else if (result.success) {
          setSession(result.data);
        }
      } catch {
        // silenciar
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [sessionId, evaluation]);

  if (loading) {
    return (
      <div className="flex items-center justify-center" style={{ minHeight: 'calc(100vh - 60px)' }}>
        <Spinner size="lg" />
      </div>
    );
  }

  if (!evaluation) {
    return (
      <div className="max-w-xl mx-auto">
        <div className="card text-center py-10">
          <AlertTriangle size={32} className="text-neutral-300 mx-auto mb-3" />
          <p className="text-sm text-neutral-500">Evaluacion no disponible.</p>
          <p className="text-xs text-neutral-400 mt-1">
            La entrevista aun no ha sido finalizada o la evaluacion no se genero correctamente.
          </p>
          <button onClick={() => navigate('/interview')} className="btn btn-secondary mt-4">
            <ArrowLeft size={15} />
            Volver
          </button>
        </div>
      </div>
    );
  }

  const score = evaluation.puntuacionGeneral ?? 0;

  const getScoreGradient = (pct) => {
    if (pct >= 70) return 'linear-gradient(135deg, #059669 0%, #10B981 50%, #34D399 100%)';
    if (pct >= 50) return 'linear-gradient(135deg, #D97706 0%, #F59E0B 50%, #FBBF24 100%)';
    return 'linear-gradient(135deg, #DC2626 0%, #EF4444 50%, #F87171 100%)';
  };

  const getRecomendacionStyle = (rec) => {
    const r = (rec || '').toLowerCase();
    if (r === 'apto') return 'bg-green-50 text-green-700 border-green-200';
    if (r.includes('reserva')) return 'bg-yellow-50 text-yellow-700 border-yellow-200';
    return 'bg-red-50 text-red-700 border-red-200';
  };

  const getRecomendacionIcon = (rec) => {
    const r = (rec || '').toLowerCase();
    if (r === 'apto') return CheckCircle2;
    if (r.includes('reserva')) return AlertTriangle;
    return XCircle;
  };

  const RecomIcon = getRecomendacionIcon(evaluation.recomendacion);

  const getBarColor = (val) => {
    if (val >= 4) return 'bg-green-500';
    if (val >= 3) return 'bg-yellow-500';
    return 'bg-red-500';
  };

  return (
    <div className="max-w-3xl mx-auto space-y-5">
      {/* Score hero */}
      <div
        className="rounded-xl p-6 text-center relative overflow-hidden"
        style={{ background: getScoreGradient(score) }}
      >
        <div className="absolute -top-10 -right-10 w-36 h-36 rounded-full bg-white/10" />
        <div className="absolute -bottom-8 -left-8 w-28 h-28 rounded-full bg-white/10" />
        <div className="relative z-10">
          <div className="flex items-center justify-center w-12 h-12 rounded-xl bg-white/20 backdrop-blur mx-auto mb-3">
            <Trophy size={24} className="text-white" />
          </div>
          <p className="text-5xl font-bold text-white">{Math.round(score)}%</p>
          <p className="text-white/80 text-sm mt-2">{evaluation.resumenGeneral}</p>
          <span className="inline-block mt-3 px-3 py-1 rounded-full text-xs font-semibold bg-white/20 text-white backdrop-blur">
            Nivel evaluado: {evaluation.nivelEvaluado}
          </span>
        </div>
      </div>

      {/* Recomendacion */}
      <div className={`card flex items-center gap-4 border ${getRecomendacionStyle(evaluation.recomendacion)}`}>
        <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-white shadow-sm">
          <RecomIcon size={20} />
        </div>
        <div>
          <p className="text-sm font-semibold">Recomendacion</p>
          <p className="text-lg font-bold">{evaluation.recomendacion}</p>
        </div>
      </div>

      {/* Competencias */}
      {evaluation.competencias && evaluation.competencias.length > 0 && (
        <div className="card p-0 overflow-hidden">
          <div className="px-5 py-3 border-b border-neutral-200 flex items-center gap-2">
            <Award size={15} className="text-primary-500" />
            <p className="text-sm font-semibold text-neutral-900">Competencias evaluadas</p>
          </div>
          <div className="divide-y divide-neutral-100">
            {evaluation.competencias.map((comp, idx) => (
              <div key={idx} className="px-5 py-4">
                <div className="flex items-center justify-between mb-2">
                  <p className="text-sm font-medium text-neutral-800">{comp.nombre}</p>
                  <div className="flex items-center gap-1">
                    {[1, 2, 3, 4, 5].map((n) => (
                      <Star
                        key={n}
                        size={14}
                        className={n <= comp.puntuacion ? 'text-yellow-400 fill-yellow-400' : 'text-neutral-200'}
                      />
                    ))}
                    <span className="ml-1 text-xs font-semibold text-neutral-600">{comp.puntuacion}/5</span>
                  </div>
                </div>
                <div className="w-full bg-neutral-100 rounded-full h-1.5 mb-2">
                  <div
                    className={`h-1.5 rounded-full transition-all ${getBarColor(comp.puntuacion)}`}
                    style={{ width: `${(comp.puntuacion / 5) * 100}%` }}
                  />
                </div>
                <p className="text-xs text-neutral-500">{comp.justificacion}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Fortalezas y areas de mejora */}
      <div className="grid grid-cols-2 gap-4">
        {/* Fortalezas */}
        {evaluation.fortalezas && evaluation.fortalezas.length > 0 && (
          <div className="card">
            <div className="flex items-center gap-2 mb-3">
              <TrendingUp size={15} className="text-green-500" />
              <p className="text-sm font-semibold text-neutral-900">Fortalezas</p>
            </div>
            <ul className="space-y-2">
              {evaluation.fortalezas.map((f, i) => (
                <li key={i} className="flex items-start gap-2 text-sm text-neutral-700">
                  <CheckCircle2 size={14} className="text-green-500 shrink-0 mt-0.5" />
                  {f}
                </li>
              ))}
            </ul>
          </div>
        )}

        {/* Areas de mejora */}
        {evaluation.areasDeMejora && evaluation.areasDeMejora.length > 0 && (
          <div className="card">
            <div className="flex items-center gap-2 mb-3">
              <TrendingDown size={15} className="text-orange-500" />
              <p className="text-sm font-semibold text-neutral-900">Areas de mejora</p>
            </div>
            <ul className="space-y-2">
              {evaluation.areasDeMejora.map((a, i) => (
                <li key={i} className="flex items-start gap-2 text-sm text-neutral-700">
                  <AlertTriangle size={14} className="text-orange-500 shrink-0 mt-0.5" />
                  {a}
                </li>
              ))}
            </ul>
          </div>
        )}
      </div>

      {/* Actions */}
      <div className="flex gap-3 justify-center">
        <button onClick={() => navigate('/interview')} className="btn btn-secondary">
          <ArrowLeft size={15} />
          Mis entrevistas
        </button>
        <button onClick={() => navigate('/interview/setup')} className="btn btn-primary">
          <RotateCcw size={15} />
          Nueva entrevista
        </button>
      </div>
    </div>
  );
}
