import { useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { CheckCircle2, XCircle, BarChart3, RotateCcw, ArrowLeft, Trophy, Target, TrendingUp } from 'lucide-react';

export default function TestStats() {
  const location = useLocation();
  const navigate = useNavigate();
  const { user } = useAuth();

  const result = location.state?.result;

  const stats = result || {
    totalPreguntas: 15,
    correctas: 0,
    incorrectas: 0,
    porcentajeAcierto: 0,
    detalle: [],
  };

  const getScoreGradient = (pct) => {
    if (pct >= 80) return 'linear-gradient(135deg, #059669 0%, #10B981 50%, #34D399 100%)';
    if (pct >= 50) return 'linear-gradient(135deg, #D97706 0%, #F59E0B 50%, #FBBF24 100%)';
    return 'linear-gradient(135deg, #DC2626 0%, #EF4444 50%, #F87171 100%)';
  };

  const getScoreBadge = (pct) => {
    if (pct >= 80) return 'badge badge-success';
    if (pct >= 50) return 'badge badge-warning';
    return 'badge badge-error';
  };

  const getScoreMessage = (pct) => {
    if (pct >= 80) return 'Dominas los conceptos evaluados.';
    if (pct >= 50) return 'Resultado aceptable. Algunos temas por reforzar.';
    return 'Se recomienda repasar los temas evaluados.';
  };

  return (
    <div className="max-w-3xl mx-auto space-y-5">
      {/* Score card — gradient hero */}
      <div className="rounded-xl p-6 text-center relative overflow-hidden" style={{ background: getScoreGradient(stats.porcentajeAcierto) }}>
        <div className="absolute -top-10 -right-10 w-36 h-36 rounded-full bg-white/10" />
        <div className="absolute -bottom-8 -left-8 w-28 h-28 rounded-full bg-white/10" />
        <div className="relative z-10">
          <div className="flex items-center justify-center w-12 h-12 rounded-xl bg-white/20 backdrop-blur mx-auto mb-3">
            <Trophy size={24} className="text-white" />
          </div>
          <p className="text-5xl font-bold text-white">
            {stats.porcentajeAcierto}%
          </p>
          <p className="text-white/80 text-sm mt-2">{getScoreMessage(stats.porcentajeAcierto)}</p>
          <span className="inline-block mt-3 px-3 py-1 rounded-full text-xs font-semibold bg-white/20 text-white backdrop-blur">
            {stats.porcentajeAcierto >= 80 ? 'Aprobado' : stats.porcentajeAcierto >= 50 ? 'Regular' : 'Reprobado'}
          </span>
          <p className="text-white/60 text-xs mt-2">{user?.nombre}</p>
        </div>
      </div>

      {/* Stats cards */}
      <div className="grid grid-cols-3 gap-3">
        <div className="card card-interactive text-center py-4">
          <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-primary-100 mx-auto mb-2">
            <Target size={17} className="text-primary-600" />
          </div>
          <p className="text-2xl font-bold text-neutral-900">{stats.totalPreguntas}</p>
          <p className="text-xs text-neutral-500 mt-0.5">Total</p>
        </div>
        <div className="card card-interactive text-center py-4">
          <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-success-light mx-auto mb-2">
            <CheckCircle2 size={17} className="text-success" />
          </div>
          <p className="text-2xl font-bold text-neutral-900">{stats.correctas}</p>
          <p className="text-xs text-neutral-500 mt-0.5">Correctas</p>
        </div>
        <div className="card card-interactive text-center py-4">
          <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-error-light mx-auto mb-2">
            <XCircle size={17} className="text-error" />
          </div>
          <p className="text-2xl font-bold text-neutral-900">{stats.incorrectas}</p>
          <p className="text-xs text-neutral-500 mt-0.5">Incorrectas</p>
        </div>
      </div>

      {/* Detail table */}
      {stats.detalle && stats.detalle.length > 0 && (
        <div className="card p-0 overflow-hidden">
          <div className="px-5 py-3 border-b border-neutral-200 flex items-center gap-2">
            <TrendingUp size={15} className="text-primary-500" />
            <p className="text-sm font-semibold text-neutral-900">Detalle por pregunta</p>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-neutral-200">
                  <th className="table-header w-12">#</th>
                  <th className="table-header text-left">Pregunta</th>
                  <th className="table-header text-center w-20">Tu resp.</th>
                  <th className="table-header text-center w-20">Correcta</th>
                  <th className="table-header text-center w-24">Resultado</th>
                </tr>
              </thead>
              <tbody>
                {stats.detalle.map((item, idx) => (
                  <tr key={item.preguntaId} className="border-b border-neutral-100 last:border-b-0 hover:bg-neutral-50 transition-colors">
                    <td className="table-cell text-neutral-500">{idx + 1}</td>
                    <td className="table-cell text-neutral-900 max-w-xs truncate">
                      {item.textoPregunta}
                    </td>
                    <td className="table-cell text-center font-mono font-semibold">
                      {item.claveSeleccionada || '—'}
                    </td>
                    <td className="table-cell text-center font-mono font-semibold text-primary-600">
                      {item.claveCorrecta}
                    </td>
                    <td className="table-cell text-center">
                      {item.esCorrecta ? (
                        <span className="badge badge-success">Correcta</span>
                      ) : (
                        <span className="badge badge-error">Incorrecta</span>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* No results placeholder */}
      {(!stats.detalle || stats.detalle.length === 0) && (
        <div className="card text-center py-10">
          <BarChart3 size={32} className="text-neutral-300 mx-auto mb-3" />
          <p className="text-sm text-neutral-500">No hay resultados disponibles.</p>
          <p className="text-xs text-neutral-400 mt-1">
            Completa el test para ver el detalle aqui.
          </p>
        </div>
      )}

      {/* Actions */}
      <div className="flex gap-3 justify-center">
        <button onClick={() => navigate('/test')} className="btn btn-secondary">
          <ArrowLeft size={15} />
          Volver al inicio
        </button>
        <button onClick={() => navigate('/test/questions')} className="btn btn-primary">
          <RotateCcw size={15} />
          Repetir test
        </button>
      </div>
    </div>
  );
}
