import { Link, Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import Spinner from '../components/UI/Spinner';
import {
  Hexagon,
  ArrowRight,
  ShieldCheck,
  ClipboardList,
  Users,
  BarChart3,
  Zap,
  Lock,
} from 'lucide-react';

export default function Landing() {
  const { isAuthenticated, isAdmin, loading } = useAuth();

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-neutral-50">
        <Spinner size="lg" />
      </div>
    );
  }

  // Si ya está autenticado, redirigir según rol
  if (isAuthenticated) {
    return <Navigate to={isAdmin ? '/dashboard' : '/test'} replace />;
  }

  const features = [
    {
      icon: ClipboardList,
      color: 'bg-primary-100 text-primary-600',
      title: 'Evaluaciones tecnicas',
      description: 'Tests de opcion multiple para medir conocimientos tecnicos de forma objetiva.',
    },
    {
      icon: Users,
      color: 'bg-accent-100 text-accent-600',
      title: 'Gestion de usuarios',
      description: 'Administra usuarios, roles y permisos desde un panel centralizado.',
    },
    {
      icon: BarChart3,
      color: 'bg-info-light text-info',
      title: 'Estadisticas detalladas',
      description: 'Visualiza resultados y metricas de rendimiento al instante.',
    },
    {
      icon: ShieldCheck,
      color: 'bg-success-light text-success',
      title: 'Seguridad integrada',
      description: 'Autenticacion JWT, refresh tokens y control de acceso basado en roles.',
    },
    {
      icon: Zap,
      color: 'bg-warning-light text-warning',
      title: 'Rapido y moderno',
      description: 'Construido con React y .NET 8 para una experiencia fluida y veloz.',
    },
    {
      icon: Lock,
      color: 'bg-primary-50 text-primary-500',
      title: 'Auditoria completa',
      description: 'Registro de acciones y eventos para trazabilidad y cumplimiento.',
    },
  ];

  return (
    <div className="min-h-screen bg-neutral-50">
      {/* ── Navbar ──────────────────────────────────────── */}
      <nav className="sticky top-0 z-50 bg-white/80 backdrop-blur border-b border-neutral-200">
        <div className="max-w-6xl mx-auto px-6 h-14 flex items-center justify-between">
          <Link to="/" className="flex items-center gap-2">
            <div className="flex items-center justify-center w-8 h-8 rounded-lg gradient-accent">
              <Hexagon size={16} className="text-white" />
            </div>
            <span className="text-base font-bold text-neutral-900">UsersApp</span>
          </Link>

          <div className="flex items-center gap-3">
            <Link to="/login" className="btn btn-ghost btn-sm">
              Iniciar sesion
            </Link>
            <Link to="/register" className="btn btn-primary btn-sm">
              Registrate
            </Link>
          </div>
        </div>
      </nav>

      {/* ── Hero ────────────────────────────────────────── */}
      <section className="relative overflow-hidden">
        <div className="gradient-hero">
          {/* Decorative circles */}
          <div className="absolute -top-24 -left-24 w-80 h-80 rounded-full bg-white/5" />
          <div className="absolute top-1/2 -right-20 w-96 h-96 rounded-full bg-white/5" />
          <div className="absolute bottom-0 left-1/3 w-48 h-48 rounded-full bg-white/5" />

          <div className="relative z-10 max-w-3xl mx-auto text-center px-6 py-20 sm:py-28">
            <div className="inline-flex items-center justify-center w-16 h-16 rounded-2xl bg-white/10 backdrop-blur mb-6">
              <Hexagon size={32} className="text-white" />
            </div>

            <h1 className="text-3xl sm:text-4xl font-extrabold text-white leading-tight mb-4">
              Plataforma de evaluaciones
              <br />
              <span className="text-primary-200">y gestion de equipos</span>
            </h1>

            <p className="text-primary-200 text-sm sm:text-base max-w-lg mx-auto mb-8 leading-relaxed">
              Evalua conocimientos tecnicos, gestiona usuarios y obtiene estadisticas en tiempo real — todo en un solo lugar.
            </p>

            <div className="flex flex-col sm:flex-row items-center justify-center gap-3">
              <Link to="/register" className="btn btn-lg bg-white text-primary-700 font-semibold hover:bg-primary-50 border-white shadow-lg">
                Comenzar gratis
                <ArrowRight size={18} />
              </Link>
              <Link to="/login" className="btn btn-lg bg-white/10 text-white border-white/20 backdrop-blur hover:bg-white/20">
                Ya tengo cuenta
              </Link>
            </div>
          </div>
        </div>

        {/* Wave separator */}
        <div className="h-12 bg-neutral-50 -mt-1">
          <svg viewBox="0 0 1440 48" fill="none" className="w-full h-full" preserveAspectRatio="none">
            <path d="M0 0C360 48 1080 48 1440 0V48H0V0Z" fill="var(--color-neutral-50)" />
          </svg>
        </div>
      </section>

      {/* ── Features ────────────────────────────────────── */}
      <section className="max-w-5xl mx-auto px-6 py-12 sm:py-16">
        <div className="text-center mb-10">
          <h2 className="text-xl sm:text-2xl font-bold text-neutral-900 mb-2">
            Todo lo que necesitas
          </h2>
          <p className="text-sm text-neutral-500 max-w-md mx-auto">
            Herramientas pensadas para simplificar evaluaciones y la gestion de tu equipo.
          </p>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {features.map((feature, idx) => (
            <div
              key={idx}
              className="card card-interactive animate-fade-in"
              style={{ animationDelay: `${idx * 60}ms` }}
            >
              <div className={`inline-flex items-center justify-center w-10 h-10 rounded-xl mb-3 ${feature.color}`}>
                <feature.icon size={20} />
              </div>
              <h3 className="text-sm font-semibold text-neutral-900 mb-1">{feature.title}</h3>
              <p className="text-xs text-neutral-500 leading-relaxed">{feature.description}</p>
            </div>
          ))}
        </div>
      </section>

      {/* ── CTA ─────────────────────────────────────────── */}
      <section className="max-w-3xl mx-auto px-6 pb-16">
        <div className="gradient-hero rounded-xl p-8 sm:p-10 text-center relative overflow-hidden">
          <div className="absolute -top-10 -right-10 w-36 h-36 rounded-full bg-white/5" />
          <div className="absolute -bottom-8 -left-8 w-28 h-28 rounded-full bg-white/5" />

          <div className="relative z-10">
            <h2 className="text-lg sm:text-xl font-bold text-white mb-2">
              Listo para empezar?
            </h2>
            <p className="text-primary-200 text-sm mb-6 max-w-sm mx-auto">
              Crea tu cuenta en segundos y accede a todas las funcionalidades.
            </p>
            <Link to="/register" className="btn bg-white text-primary-700 font-semibold hover:bg-primary-50 border-white shadow-lg">
              Crear cuenta
              <ArrowRight size={16} />
            </Link>
          </div>
        </div>
      </section>

      {/* ── Footer ──────────────────────────────────────── */}
      <footer className="border-t border-neutral-200 bg-white">
        <div className="max-w-6xl mx-auto px-6 py-6 flex flex-col sm:flex-row items-center justify-between gap-3">
          <div className="flex items-center gap-2">
            <div className="flex items-center justify-center w-6 h-6 rounded gradient-accent">
              <Hexagon size={12} className="text-white" />
            </div>
            <span className="text-xs font-semibold text-neutral-700">UsersApp</span>
          </div>
          <p className="text-xs text-neutral-400">
            &copy; {new Date().getFullYear()} UsersApp. Proyecto de evaluacion tecnica.
          </p>
        </div>
      </footer>
    </div>
  );
}
