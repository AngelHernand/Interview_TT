import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import interviewService from '../services/interviewService';
import Spinner from '../components/UI/Spinner';
import Pagination from '../components/UI/Pagination';
import {
  BrainCircuit,
  Plus,
  MessageSquare,
  Clock,
  CheckCircle2,
  XCircle,
  ChevronRight,
} from 'lucide-react';

export default function InterviewHistory() {
  const { user } = useAuth();
  const navigate = useNavigate();

  const [sessions, setSessions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      try {
        const result = await interviewService.getUserSessions(page, 10);
        if (result.success) {
          setSessions(result.data.items || []);
          setTotalPages(result.data.totalPages || 1);
        }
      } catch {
        // error manejado por interceptor
      } finally {
        setLoading(false);
      }
    };

    load();
  }, [page]);

  const getEstadoBadge = (estado) => {
    if (estado === 'EnCurso') return 'badge badge-warning';
    if (estado === 'Finalizada') return 'badge badge-success';
    return 'badge badge-error';
  };

  const getScoreColor = (score) => {
    if (score == null) return 'text-neutral-400';
    if (score >= 70) return 'text-green-600';
    if (score >= 50) return 'text-yellow-600';
    return 'text-red-600';
  };

  const handleClick = (s) => {
    if (s.estado === 'EnCurso') navigate(`/interview/${s.id}`);
    else navigate(`/interview/${s.id}/results`);
  };

  return (
    <div className="max-w-3xl mx-auto space-y-5">
      {/* Hero */}
      <div className="gradient-hero rounded-xl p-6 relative overflow-hidden">
        <div className="absolute -top-10 -right-10 w-36 h-36 rounded-full bg-white/5" />
        <div className="absolute -bottom-6 -left-6 w-24 h-24 rounded-full bg-white/5" />
        <div className="relative z-10 flex items-center justify-between">
          <div className="flex items-center gap-4">
            <div className="flex items-center justify-center w-12 h-12 rounded-xl bg-white/10 backdrop-blur">
              <BrainCircuit size={24} className="text-white" />
            </div>
            <div>
              <h1 className="text-lg font-bold text-white">Mis entrevistas</h1>
              <p className="text-primary-200 text-sm">
                Historial de entrevistas simuladas, {user?.nombre}.
              </p>
            </div>
          </div>
          <button
            onClick={() => navigate('/interview/setup')}
            className="flex items-center gap-1.5 px-4 py-2 rounded-lg bg-white/10 backdrop-blur text-white text-sm font-medium hover:bg-white/20 transition-colors"
          >
            <Plus size={16} />
            Nueva
          </button>
        </div>
      </div>

      {/* List */}
      {loading ? (
        <div className="card py-10">
          <Spinner size="lg" />
        </div>
      ) : sessions.length === 0 ? (
        <div className="card text-center py-10">
          <MessageSquare size={32} className="text-neutral-300 mx-auto mb-3" />
          <p className="text-sm text-neutral-500">No tienes entrevistas aun.</p>
          <button
            onClick={() => navigate('/interview/setup')}
            className="btn btn-primary mt-4"
          >
            <Plus size={15} />
            Iniciar primera entrevista
          </button>
        </div>
      ) : (
        <div className="space-y-3">
          {sessions.map((s) => (
            <button
              key={s.id}
              onClick={() => handleClick(s)}
              className="card card-interactive w-full text-left flex items-center gap-4"
            >
              <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary-100 shrink-0">
                <BrainCircuit size={18} className="text-primary-600" />
              </div>

              <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2">
                  <p className="text-sm font-semibold text-neutral-900 capitalize">
                    {s.tipoEntrevista}
                    {s.tecnologia && ` - ${s.tecnologia}`}
                  </p>
                  <span className={getEstadoBadge(s.estado)}>{s.estado}</span>
                </div>
                <div className="flex items-center gap-3 mt-1 text-xs text-neutral-500">
                  <span>Nivel: {s.nivel}</span>
                  <span className="flex items-center gap-0.5">
                    <Clock size={11} />
                    {new Date(s.fechaInicio).toLocaleDateString('es')}
                  </span>
                  <span className="flex items-center gap-0.5">
                    <MessageSquare size={11} />
                    {s.totalMensajes} msgs
                  </span>
                </div>
              </div>

              {/* Score */}
              <div className="shrink-0 text-right">
                {s.puntuacionGeneral != null ? (
                  <p className={`text-lg font-bold ${getScoreColor(s.puntuacionGeneral)}`}>
                    {Math.round(s.puntuacionGeneral)}%
                  </p>
                ) : s.estado === 'EnCurso' ? (
                  <span className="text-xs text-neutral-400">En curso</span>
                ) : (
                  <span className="text-xs text-neutral-400">-</span>
                )}
              </div>

              <ChevronRight size={16} className="text-neutral-300 shrink-0" />
            </button>
          ))}
        </div>
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <Pagination
          currentPage={page}
          totalPages={totalPages}
          onPageChange={setPage}
        />
      )}
    </div>
  );
}
