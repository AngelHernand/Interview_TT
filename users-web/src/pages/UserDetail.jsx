import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import userService from '../services/userService';
import Spinner from '../components/UI/Spinner';
import toast from 'react-hot-toast';
import { ArrowLeft, Pencil, Lock, Unlock, Calendar, Clock, Hash } from 'lucide-react';

export default function UserDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const result = await userService.getById(id);
        if (result.success) {
          setUser(result.data);
        } else {
          toast.error('Usuario no encontrado');
          navigate('/users');
        }
      } catch {
        toast.error('Error al cargar usuario');
        navigate('/users');
      } finally {
        setLoading(false);
      }
    };
    fetchUser();
  }, [id, navigate]);

  const handleToggleBlock = async () => {
    try {
      const result = await userService.toggleBlock(id);
      if (result.success) {
        toast.success(result.message);
        setUser((prev) => ({ ...prev, bloqueado: !prev.bloqueado }));
      }
    } catch {
      toast.error('Error al cambiar estado');
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <Spinner size="lg" />
      </div>
    );
  }

  if (!user) return null;

  return (
    <div className="max-w-2xl mx-auto">
      <button
        onClick={() => navigate('/users')}
        className="btn btn-ghost btn-sm mb-5 -ml-2 text-neutral-500"
      >
        <ArrowLeft size={14} />
        Usuarios
      </button>

      <div className="card">
        {/* Header */}
        <div className="flex items-start justify-between mb-6">
          <div className="flex items-center gap-4">
            <div
              className="shrink-0 flex items-center justify-center rounded-xl text-white font-bold"
              style={{ width: 56, height: 56, fontSize: 20, background: 'linear-gradient(135deg, var(--color-primary-400), var(--color-accent-500))' }}
            >
              {user.nombre?.charAt(0)?.toUpperCase()}
            </div>
            <div>
              <h1 className="text-base font-bold text-neutral-900">{user.nombre}</h1>
              <p className="text-sm text-neutral-500">{user.email}</p>
              <div className="flex items-center gap-2 mt-2">
                <span className={`badge ${user.rol === 'Admin' ? 'badge-info' : 'badge-default'}`}>{user.rol}</span>
                {user.bloqueado ? (
                  <span className="badge badge-error">Bloqueado</span>
                ) : user.activo ? (
                  <span className="badge badge-success">Activo</span>
                ) : (
                  <span className="badge badge-warning">Inactivo</span>
                )}
              </div>
            </div>
          </div>
          <Link to={`/users/${id}/edit`} className="btn btn-secondary btn-sm">
            <Pencil size={13} />
            Editar
          </Link>
        </div>

        {/* Metadata */}
        <div className="border-t border-neutral-200 pt-5 grid grid-cols-2 gap-y-4 gap-x-8">
          <div className="flex items-start gap-2">
            <Hash size={14} className="text-neutral-400 mt-0.5" />
            <div>
              <p className="text-[11px] font-medium text-neutral-500 uppercase tracking-wider mb-1">ID de usuario</p>
              <p className="text-sm font-mono text-neutral-700">{user.id}</p>
            </div>
          </div>
          <div className="flex items-start gap-2">
            <Calendar size={14} className="text-neutral-400 mt-0.5" />
            <div>
              <p className="text-[11px] font-medium text-neutral-500 uppercase tracking-wider mb-1">Fecha de registro</p>
              <p className="text-sm text-neutral-900">
                {user.fechaCreacion ? new Date(user.fechaCreacion).toLocaleDateString() : '-'}
              </p>
            </div>
          </div>
          <div className="flex items-start gap-2">
            <Clock size={14} className="text-neutral-400 mt-0.5" />
            <div>
              <p className="text-[11px] font-medium text-neutral-500 uppercase tracking-wider mb-1">Ultimo acceso</p>
              <p className="text-sm text-neutral-900">
                {user.ultimoAcceso ? new Date(user.ultimoAcceso).toLocaleString() : 'Nunca'}
              </p>
            </div>
          </div>
        </div>

        {/* Admin actions */}
        {isAdmin && (
          <div className="border-t border-neutral-200 mt-6 pt-5">
            <p className="text-[11px] font-medium text-neutral-500 uppercase tracking-wider mb-3">Acciones de administrador</p>
            <button
              onClick={handleToggleBlock}
              className={`btn btn-sm ${user.bloqueado ? 'btn-primary' : 'btn-danger'}`}
            >
              {user.bloqueado ? <Unlock size={13} /> : <Lock size={13} />}
              {user.bloqueado ? 'Desbloquear usuario' : 'Bloquear usuario'}
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
