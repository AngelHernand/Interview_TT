import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import toast from 'react-hot-toast';
import interviewService from '../services/interviewService';
import Spinner from '../components/UI/Spinner';
import {
  MessageSquare,
  ArrowRight,
  Code2,
  Users,
  Shuffle,
  Clock,
  Zap,
  BrainCircuit,
} from 'lucide-react';

const TIPOS = [
  { value: 'tecnica', label: 'Tecnica', icon: Code2, desc: 'Preguntas sobre tecnologias y conceptos.' },
  { value: 'behavioral', label: 'Behavioral', icon: Users, desc: 'Competencias conductuales y situacionales.' },
  { value: 'mixta', label: 'Mixta', icon: Shuffle, desc: 'Combina preguntas tecnicas y conductuales.' },
];

const TECNOLOGIAS = ['.NET', 'C#', 'SQL', 'React', 'JavaScript', 'Python'];

const NIVELES = [
  { value: 'Junior', desc: 'Fundamentos y conceptos basicos' },
  { value: 'Mid', desc: 'Diseno, patrones y trade-offs' },
  { value: 'Senior', desc: 'Arquitectura y liderazgo tecnico' },
];

const DURACIONES = [15, 30, 45, 60];

export default function InterviewSetup() {
  const { user } = useAuth();
  const navigate = useNavigate();

  const [tipo, setTipo] = useState('tecnica');
  const [tecnologia, setTecnologia] = useState('.NET');
  const [nivel, setNivel] = useState('Junior');
  const [duracion, setDuracion] = useState(30);
  const [loading, setLoading] = useState(false);

  const needsTech = tipo === 'tecnica' || tipo === 'mixta';

  const handleStart = async () => {
    setLoading(true);
    try {
      const result = await interviewService.startInterview({
        tipoEntrevista: tipo,
        tecnologia: needsTech ? tecnologia : null,
        nivel,
        duracionMinutos: duracion,
      });

      if (result.success) {
        toast.success('Entrevista iniciada');
        navigate(`/interview/${result.data.id}`);
      } else {
        toast.error(result.message || 'Error al iniciar la entrevista');
      }
    } catch {
      toast.error('Error al conectar con el servidor');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-xl mx-auto space-y-5">
      {/* Hero */}
      <div className="gradient-hero rounded-xl p-6 relative overflow-hidden">
        <div className="absolute -top-10 -right-10 w-36 h-36 rounded-full bg-white/5" />
        <div className="absolute -bottom-6 -left-6 w-24 h-24 rounded-full bg-white/5" />
        <div className="relative z-10 flex items-center gap-4">
          <div className="flex items-center justify-center w-12 h-12 rounded-xl bg-white/10 backdrop-blur">
            <BrainCircuit size={24} className="text-white" />
          </div>
          <div>
            <h1 className="text-lg font-bold text-white">Entrevista simulada con IA</h1>
            <p className="text-primary-200 text-sm">
              Hola, {user?.nombre}. Configura tu entrevista.
            </p>
          </div>
        </div>
      </div>

      {/* Form */}
      <div className="card space-y-5">
        {/* Tipo */}
        <div>
          <p className="text-sm font-semibold text-neutral-900 mb-3">Tipo de entrevista</p>
          <div className="grid grid-cols-3 gap-3">
            {TIPOS.map((t) => (
              <button
                key={t.value}
                onClick={() => setTipo(t.value)}
                className={`p-3 rounded-lg border-2 text-left transition-all ${
                  tipo === t.value
                    ? 'border-primary-500 bg-primary-50'
                    : 'border-neutral-200 hover:border-neutral-300'
                }`}
              >
                <t.icon
                  size={18}
                  className={tipo === t.value ? 'text-primary-600' : 'text-neutral-400'}
                />
                <p className={`text-sm font-semibold mt-2 ${
                  tipo === t.value ? 'text-primary-700' : 'text-neutral-700'
                }`}>
                  {t.label}
                </p>
                <p className="text-[11px] text-neutral-500 mt-0.5">{t.desc}</p>
              </button>
            ))}
          </div>
        </div>

        {/* Tecnologia */}
        <div className={!needsTech ? 'opacity-40 pointer-events-none' : ''}>
          <p className="text-sm font-semibold text-neutral-900 mb-2">Tecnologia</p>
          <div className="flex flex-wrap gap-2">
            {TECNOLOGIAS.map((tech) => (
              <button
                key={tech}
                onClick={() => setTecnologia(tech)}
                className={`px-3 py-1.5 rounded-lg text-sm font-medium border transition-all ${
                  tecnologia === tech && needsTech
                    ? 'border-primary-500 bg-primary-50 text-primary-700'
                    : 'border-neutral-200 text-neutral-600 hover:border-neutral-300'
                }`}
              >
                {tech}
              </button>
            ))}
          </div>
        </div>

        {/* Nivel */}
        <div>
          <p className="text-sm font-semibold text-neutral-900 mb-2">Nivel</p>
          <div className="space-y-2">
            {NIVELES.map((n) => (
              <button
                key={n.value}
                onClick={() => setNivel(n.value)}
                className={`w-full flex items-center gap-3 p-3 rounded-lg border-2 text-left transition-all ${
                  nivel === n.value
                    ? 'border-primary-500 bg-primary-50'
                    : 'border-neutral-200 hover:border-neutral-300'
                }`}
              >
                <Zap
                  size={16}
                  className={nivel === n.value ? 'text-primary-600' : 'text-neutral-400'}
                />
                <div>
                  <p className={`text-sm font-semibold ${
                    nivel === n.value ? 'text-primary-700' : 'text-neutral-700'
                  }`}>
                    {n.value}
                  </p>
                  <p className="text-[11px] text-neutral-500">{n.desc}</p>
                </div>
              </button>
            ))}
          </div>
        </div>

        {/* Duracion */}
        <div>
          <p className="text-sm font-semibold text-neutral-900 mb-2">
            <Clock size={14} className="inline mr-1.5 -mt-0.5" />
            Duracion: {duracion} min
          </p>
          <div className="flex gap-2">
            {DURACIONES.map((d) => (
              <button
                key={d}
                onClick={() => setDuracion(d)}
                className={`flex-1 py-2 rounded-lg text-sm font-medium border transition-all ${
                  duracion === d
                    ? 'border-primary-500 bg-primary-50 text-primary-700'
                    : 'border-neutral-200 text-neutral-600 hover:border-neutral-300'
                }`}
              >
                {d} min
              </button>
            ))}
          </div>
        </div>

        {/* Submit */}
        <button
          onClick={handleStart}
          disabled={loading}
          className="btn btn-primary w-full"
        >
          {loading ? (
            <Spinner size="sm" />
          ) : (
            <>
              <MessageSquare size={16} />
              Iniciar entrevista
              <ArrowRight size={16} />
            </>
          )}
        </button>
      </div>
    </div>
  );
}
