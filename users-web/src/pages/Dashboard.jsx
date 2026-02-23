import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import userService from '../services/userService';
import Spinner from '../components/UI/Spinner';
import { Users, ShieldCheck, Mail, ArrowRight, UserPlus, ClipboardList } from 'lucide-react';

export default function Dashboard() {
  const { user, isAdmin } = useAuth();
  const [stats, setStats] = useState({ totalUsers: 0 });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const result = await userService.getAll(1, 1);
        if (result.success) {
          setStats({ totalUsers: result.data.totalCount });
        }
      } catch (error) {
        console.error('Error fetching stats:', error);
      } finally {
        setLoading(false);
      }
    };
    fetchStats();
  }, []);

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <Spinner size="lg" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Welcome banner */}
      <div className="gradient-hero rounded-xl p-6 relative overflow-hidden">
        <div className="absolute -top-10 -right-10 w-40 h-40 rounded-full bg-white/5" />
        <div className="absolute -bottom-8 -right-4 w-28 h-28 rounded-full bg-white/5" />
        <div className="relative z-10">
          <h1 className="text-xl font-bold text-white">
            Bienvenido, {user?.nombre}
          </h1>
          <p className="text-primary-200 text-sm mt-1">
            {isAdmin ? 'Panel de administracion — gestiona usuarios y configuraciones.' : 'Tu panel personal — accede a tus evaluaciones.'}
          </p>
        </div>
      </div>

      {/* Stat cards */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div className="card card-interactive flex items-center gap-4">
          <div className="flex items-center justify-center w-11 h-11 rounded-xl bg-primary-100">
            <Users size={20} className="text-primary-600" />
          </div>
          <div>
            <p className="text-[11px] font-medium text-neutral-500 uppercase tracking-wider">
              Total usuarios
            </p>
            <p className="text-2xl font-bold text-neutral-900">{stats.totalUsers}</p>
          </div>
        </div>

        <div className="card card-interactive flex items-center gap-4">
          <div className="flex items-center justify-center w-11 h-11 rounded-xl bg-accent-100">
            <ShieldCheck size={20} className="text-accent-600" />
          </div>
          <div>
            <p className="text-[11px] font-medium text-neutral-500 uppercase tracking-wider">
              Tu rol
            </p>
            <p className="text-2xl font-bold text-neutral-900">{user?.rol}</p>
          </div>
        </div>

        <div className="card card-interactive flex items-center gap-4">
          <div className="flex items-center justify-center w-11 h-11 rounded-xl bg-info-light">
            <Mail size={20} className="text-info" />
          </div>
          <div>
            <p className="text-[11px] font-medium text-neutral-500 uppercase tracking-wider">
              Email
            </p>
            <p className="text-sm font-medium text-neutral-900 truncate max-w-[180px]">
              {user?.email}
            </p>
          </div>
        </div>
      </div>

      {/* Quick actions */}
      <div>
        <h2 className="text-sm font-semibold text-neutral-900 mb-3">Accesos rapidos</h2>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
          <Link
            to="/users"
            className="card card-interactive flex items-center justify-between p-4 group"
          >
            <div className="flex items-center gap-3">
              <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-primary-50 group-hover:bg-primary-100 transition-colors">
                <Users size={16} className="text-primary-600" />
              </div>
              <div>
                <p className="text-sm font-medium text-neutral-900">Ver usuarios</p>
                <p className="text-xs text-neutral-500">Lista completa</p>
              </div>
            </div>
            <ArrowRight size={14} className="text-neutral-400 group-hover:text-primary-500 transition-colors" />
          </Link>

          <Link
            to="/profile"
            className="card card-interactive flex items-center justify-between p-4 group"
          >
            <div className="flex items-center gap-3">
              <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-accent-50 group-hover:bg-accent-100 transition-colors">
                <ClipboardList size={16} className="text-accent-600" />
              </div>
              <div>
                <p className="text-sm font-medium text-neutral-900">Mi perfil</p>
                <p className="text-xs text-neutral-500">Ver y editar informacion</p>
              </div>
            </div>
            <ArrowRight size={14} className="text-neutral-400 group-hover:text-accent-500 transition-colors" />
          </Link>

          {isAdmin && (
            <Link
              to="/users/new"
              className="card card-interactive flex items-center justify-between p-4 group"
              style={{ borderColor: 'var(--color-primary-200)', background: 'var(--color-primary-50)' }}
            >
              <div className="flex items-center gap-3">
                <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-primary-200">
                  <UserPlus size={16} className="text-primary-700" />
                </div>
                <div>
                  <p className="text-sm font-medium text-primary-900">Crear usuario</p>
                  <p className="text-xs text-primary-600">Agregar nuevo registro</p>
                </div>
              </div>
              <ArrowRight size={14} className="text-primary-400 group-hover:text-primary-600 transition-colors" />
            </Link>
          )}
        </div>
      </div>
    </div>
  );
}
