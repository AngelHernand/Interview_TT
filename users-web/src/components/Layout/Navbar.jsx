import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import {
  LayoutDashboard,
  Users,
  UserPlus,
  ClipboardList,
  BarChart3,
  User,
  LogOut,
  Hexagon,
  BrainCircuit,
  MessageSquare,
} from 'lucide-react';

export default function Sidebar() {
  const { user, logout, isAdmin } = useAuth();
  const location = useLocation();

  const isActive = (path) =>
    path === '/'
      ? location.pathname === path
      : location.pathname.startsWith(path);

  const linkClass = (path) =>
    `flex items-center gap-3 px-3 py-2 rounded-lg text-[13px] font-medium transition-all duration-150 ${
      isActive(path)
        ? 'bg-white/10 text-white shadow-sm'
        : 'text-neutral-400 hover:bg-white/5 hover:text-neutral-200'
    }`;

  const sectionLabel = (text) => (
    <p className="px-3 pt-5 pb-1 text-[10px] font-semibold uppercase tracking-widest text-neutral-500">
      {text}
    </p>
  );

  return (
    <aside
      className="fixed top-0 left-0 h-screen flex flex-col"
      style={{
        width: 240,
        background: 'linear-gradient(180deg, #1E293B 0%, #0F172A 100%)',
      }}
    >
      {/* Logo */}
      <div className="flex items-center gap-2.5 px-5 h-14 shrink-0" style={{ borderBottom: '1px solid rgba(255,255,255,0.08)' }}>
        <div className="flex items-center justify-center w-8 h-8 rounded-lg gradient-accent">
          <Hexagon size={16} className="text-white" />
        </div>
        <span className="text-[15px] font-bold text-white tracking-tight">
          UsersApp
        </span>
      </div>

      {/* Navigation */}
      <nav className="flex-1 overflow-y-auto px-3 py-2">
        {isAdmin ? (
          <>
            {sectionLabel('General')}
            <Link to="/dashboard" className={linkClass('/dashboard')}>
              <LayoutDashboard size={16} />
              Dashboard
            </Link>

            {sectionLabel('Usuarios')}
            <Link to="/users" className={linkClass('/users')}>
              <Users size={16} />
              Lista de usuarios
            </Link>
            <Link to="/users/new" className={linkClass('/users/new')}>
              <UserPlus size={16} />
              Crear usuario
            </Link>
          </>
        ) : (
          <>
            {sectionLabel('Evaluacion')}
            <Link to="/test" className={linkClass('/test')}>
              <ClipboardList size={16} />
              Iniciar test
            </Link>
            <Link to="/test/stats" className={linkClass('/test/stats')}>
              <BarChart3 size={16} />
              Resultados
            </Link>

            {sectionLabel('Entrevista IA')}
            <Link to="/interview/setup" className={linkClass('/interview/setup')}>
              <BrainCircuit size={16} />
              Nueva entrevista
            </Link>
            <Link to="/interview" className={linkClass('/interview')}>
              <MessageSquare size={16} />
              Mis entrevistas
            </Link>
          </>
        )}

        {sectionLabel('Cuenta')}
        <Link to="/profile" className={linkClass('/profile')}>
          <User size={16} />
          Mi perfil
        </Link>
      </nav>

      {/* User footer */}
      <div className="px-4 py-3 shrink-0" style={{ borderTop: '1px solid rgba(255,255,255,0.08)' }}>
        <div className="flex items-center gap-3">
          <div
            className="shrink-0 flex items-center justify-center rounded-full text-xs font-bold gradient-accent text-white"
            style={{ width: 32, height: 32 }}
          >
            {user?.nombre?.charAt(0)?.toUpperCase()}
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-[13px] font-medium text-neutral-200 truncate">
              {user?.nombre}
            </p>
            <p className="text-[11px] text-neutral-500 truncate">{user?.rol}</p>
          </div>
          <button
            onClick={logout}
            title="Cerrar sesion"
            className="p-1.5 rounded-md text-neutral-500 hover:text-neutral-300 hover:bg-white/5 transition-colors"
          >
            <LogOut size={16} />
          </button>
        </div>
      </div>
    </aside>
  );
}
