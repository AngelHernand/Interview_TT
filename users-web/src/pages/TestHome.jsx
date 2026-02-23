import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { ClipboardList, ArrowRight, BookOpen, Timer, BarChart3, CheckCircle2 } from 'lucide-react';

export default function TestHome() {
  const { user } = useAuth();
  const navigate = useNavigate();

  const rules = [
    { icon: BookOpen, color: 'text-primary-600 bg-primary-100', text: <>El test consta de <strong>15 preguntas</strong> de opcion multiple.</> },
    { icon: CheckCircle2, color: 'text-accent-600 bg-accent-100', text: <>Cada pregunta tiene <strong>4 opciones</strong>, solo una es correcta.</> },
    { icon: Timer, color: 'text-warning bg-warning-light', text: <>Debes avanzar pregunta por pregunta, <strong>sin retroceder</strong>.</> },
    { icon: BarChart3, color: 'text-info bg-info-light', text: <>Al finalizar veras tus <strong>estadisticas detalladas</strong>.</> },
  ];

  return (
    <div className="max-w-xl mx-auto space-y-5">
      {/* Hero header */}
      <div className="gradient-hero rounded-xl p-6 relative overflow-hidden">
        <div className="absolute -top-10 -right-10 w-36 h-36 rounded-full bg-white/5" />
        <div className="absolute -bottom-6 -left-6 w-24 h-24 rounded-full bg-white/5" />
        <div className="relative z-10 flex items-center gap-4">
          <div className="flex items-center justify-center w-12 h-12 rounded-xl bg-white/10 backdrop-blur">
            <ClipboardList size={24} className="text-white" />
          </div>
          <div>
            <h1 className="text-lg font-bold text-white">Test de conocimientos</h1>
            <p className="text-primary-200 text-sm">
              Hola, {user?.nombre}. Evalua tus conocimientos tecnicos.
            </p>
          </div>
        </div>
      </div>

      {/* Rules card */}
      <div className="card">
        <p className="text-sm font-semibold text-neutral-900 mb-4">Instrucciones</p>
        <div className="space-y-3 mb-6">
          {rules.map((rule, idx) => (
            <div key={idx} className="flex items-start gap-3">
              <div className={`flex items-center justify-center w-8 h-8 rounded-lg shrink-0 ${rule.color}`}>
                <rule.icon size={15} />
              </div>
              <p className="text-sm text-neutral-700 pt-1">{rule.text}</p>
            </div>
          ))}
        </div>

        <button
          onClick={() => navigate('/test/questions')}
          className="btn btn-primary w-full"
        >
          Iniciar test
          <ArrowRight size={16} />
        </button>
      </div>
    </div>
  );
}
