import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import userService from '../services/userService';
import authService from '../services/authService';
import Spinner from '../components/UI/Spinner';
import Modal from '../components/UI/Modal';
import toast from 'react-hot-toast';
import { Pencil, KeyRound, Shield, Calendar, Clock } from 'lucide-react';

export default function Profile() {
  const { user, updateUser } = useAuth();
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [editMode, setEditMode] = useState(false);
  const [passwordModal, setPasswordModal] = useState(false);

  const [formData, setFormData] = useState({ nombre: '', email: '' });
  const [passwordData, setPasswordData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmNewPassword: '',
  });
  const [errors, setErrors] = useState({});

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const result = await userService.getCurrentUser();
        if (result.success) {
          setProfile(result.data);
          setFormData({ nombre: result.data.nombre, email: result.data.email });
        }
      } catch (error) {
        console.error('Error fetching profile:', error);
      } finally {
        setLoading(false);
      }
    };
    fetchProfile();
  }, []);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }));
  };

  const handlePasswordChange = (e) => {
    const { name, value } = e.target;
    setPasswordData((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }));
  };

  const handleSaveProfile = async (e) => {
    e.preventDefault();
    setSaving(true);
    setErrors({});
    try {
      const result = await userService.update(profile.id, formData);
      if (result.success) {
        setProfile(result.data);
        updateUser({ ...user, nombre: result.data.nombre, email: result.data.email });
        setEditMode(false);
        toast.success('Perfil actualizado correctamente');
      }
    } catch (error) {
      toast.error(error.response?.data?.message || 'Error al actualizar perfil');
    } finally {
      setSaving(false);
    }
  };

  const handleChangePassword = async (e) => {
    e.preventDefault();
    const newErrors = {};
    if (!passwordData.currentPassword) newErrors.currentPassword = 'Requerido';
    if (!passwordData.newPassword) {
      newErrors.newPassword = 'Requerido';
    } else if (passwordData.newPassword.length < 8) {
      newErrors.newPassword = 'Minimo 8 caracteres';
    }
    if (passwordData.newPassword !== passwordData.confirmNewPassword) {
      newErrors.confirmNewPassword = 'Las contrasenas no coinciden';
    }
    if (Object.keys(newErrors).length > 0) { setErrors(newErrors); return; }

    setSaving(true);
    try {
      const result = await authService.changePassword(
        passwordData.currentPassword,
        passwordData.newPassword,
        passwordData.confirmNewPassword
      );
      if (result.success) {
        toast.success('Contrasena cambiada correctamente');
        setPasswordModal(false);
        setPasswordData({ currentPassword: '', newPassword: '', confirmNewPassword: '' });
      } else {
        toast.error(result.message);
      }
    } catch (error) {
      toast.error(error.response?.data?.message || 'Error al cambiar contrasena');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <Spinner size="lg" />
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto space-y-5">
      {/* Profile hero */}
      <div className="gradient-hero rounded-xl p-6 relative overflow-hidden">
        <div className="absolute -top-10 -right-10 w-36 h-36 rounded-full bg-white/5" />
        <div className="absolute -bottom-6 -left-6 w-24 h-24 rounded-full bg-white/5" />
        <div className="relative z-10 flex items-center gap-4">
          <div
            className="shrink-0 flex items-center justify-center rounded-xl font-bold text-white"
            style={{ width: 52, height: 52, fontSize: 20, background: 'linear-gradient(135deg, var(--color-primary-400), var(--color-accent-500))' }}
          >
            {profile?.nombre?.charAt(0)?.toUpperCase()}
          </div>
          <div>
            <h1 className="text-lg font-bold text-white">{profile?.nombre}</h1>
            <p className="text-primary-200 text-sm">{profile?.email}</p>
          </div>
        </div>
      </div>

      {/* Profile */}
      <div className="card">
        <div className="flex items-center justify-between mb-5">
          <p className="text-sm font-semibold text-neutral-900">Datos personales</p>
          {!editMode ? (
            <button onClick={() => setEditMode(true)} className="btn btn-secondary btn-sm">
              <Pencil size={13} />
              Editar
            </button>
          ) : (
            <button onClick={() => setEditMode(false)} className="btn btn-ghost btn-sm text-neutral-500">
              Cancelar
            </button>
          )}
        </div>

        {editMode ? (
          <form onSubmit={handleSaveProfile} className="space-y-4">
            <div>
              <label className="label">Nombre</label>
              <input name="nombre" value={formData.nombre} onChange={handleChange} className="input-field" />
            </div>
            <div>
              <label className="label">Email</label>
              <input name="email" type="email" value={formData.email} onChange={handleChange} className="input-field" />
            </div>
            <div className="flex justify-end gap-3 pt-3 border-t border-neutral-200">
              <button type="button" onClick={() => setEditMode(false)} className="btn btn-secondary">
                Cancelar
              </button>
              <button type="submit" disabled={saving} className="btn btn-primary">
                {saving ? <Spinner size="sm" /> : 'Guardar cambios'}
              </button>
            </div>
          </form>
        ) : (
          <div>
            <div className="flex items-center gap-4 mb-5">
              <div
                className="shrink-0 flex items-center justify-center rounded-xl text-white font-bold"
                style={{ width: 48, height: 48, fontSize: 18, background: 'linear-gradient(135deg, var(--color-primary-400), var(--color-accent-500))' }}
              >
                {profile?.nombre?.charAt(0)?.toUpperCase()}
              </div>
              <div>
                <p className="text-sm font-medium text-neutral-900">{profile?.nombre}</p>
                <p className="text-xs text-neutral-500">{profile?.email}</p>
                <span className="badge badge-info mt-1">{profile?.rol}</span>
              </div>
            </div>

            <div className="border-t border-neutral-200 pt-4 grid grid-cols-2 gap-y-3 gap-x-8">
              <div className="flex items-start gap-2">
                <Shield size={14} className="text-neutral-400 mt-0.5" />
                <div>
                  <p className="text-[11px] font-medium text-neutral-500 uppercase tracking-wider mb-0.5">Estado</p>
                  {profile?.bloqueado ? (
                    <span className="badge badge-error text-xs">Bloqueado</span>
                  ) : profile?.activo ? (
                    <span className="badge badge-success text-xs">Activo</span>
                  ) : (
                    <span className="badge badge-default text-xs">Inactivo</span>
                  )}
                </div>
              </div>
              <div className="flex items-start gap-2">
                <Calendar size={14} className="text-neutral-400 mt-0.5" />
                <div>
                  <p className="text-[11px] font-medium text-neutral-500 uppercase tracking-wider mb-0.5">Fecha de registro</p>
                  <p className="text-sm text-neutral-900">
                    {profile?.fechaCreacion ? new Date(profile.fechaCreacion).toLocaleDateString() : '-'}
                  </p>
                </div>
              </div>
              <div className="flex items-start gap-2">
                <Clock size={14} className="text-neutral-400 mt-0.5" />
                <div>
                  <p className="text-[11px] font-medium text-neutral-500 uppercase tracking-wider mb-0.5">Ultimo acceso</p>
                  <p className="text-sm text-neutral-900">
                    {profile?.ultimoAcceso ? new Date(profile.ultimoAcceso).toLocaleString() : '-'}
                  </p>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Security */}
      <div className="card card-accent-left">
        <div className="flex items-center gap-2 mb-4">
          <div className="flex items-center justify-center w-7 h-7 rounded-lg bg-accent-100">
            <KeyRound size={14} className="text-accent-600" />
          </div>
          <p className="text-sm font-semibold text-neutral-900">Seguridad</p>
        </div>
        <div className="flex items-center justify-between">
          <div>
            <p className="text-sm font-medium text-neutral-900">Contrasena</p>
            <p className="text-xs text-neutral-500">Se recomienda cambiarla periodicamente</p>
          </div>
          <button onClick={() => setPasswordModal(true)} className="btn btn-secondary btn-sm">
            <KeyRound size={13} />
            Cambiar
          </button>
        </div>
      </div>

      {/* Password modal */}
      <Modal
        isOpen={passwordModal}
        onClose={() => {
          setPasswordModal(false);
          setPasswordData({ currentPassword: '', newPassword: '', confirmNewPassword: '' });
          setErrors({});
        }}
        title="Cambiar contrasena"
        size="sm"
      >
        <form onSubmit={handleChangePassword} className="space-y-4">
          <div>
            <label className="label">Contrasena actual</label>
            <input
              name="currentPassword"
              type="password"
              value={passwordData.currentPassword}
              onChange={handlePasswordChange}
              className={`input-field ${errors.currentPassword ? 'input-error' : ''}`}
            />
            {errors.currentPassword && <p className="error-text">{errors.currentPassword}</p>}
          </div>
          <div>
            <label className="label">Nueva contrasena</label>
            <input
              name="newPassword"
              type="password"
              value={passwordData.newPassword}
              onChange={handlePasswordChange}
              className={`input-field ${errors.newPassword ? 'input-error' : ''}`}
            />
            {errors.newPassword && <p className="error-text">{errors.newPassword}</p>}
          </div>
          <div>
            <label className="label">Confirmar nueva contrasena</label>
            <input
              name="confirmNewPassword"
              type="password"
              value={passwordData.confirmNewPassword}
              onChange={handlePasswordChange}
              className={`input-field ${errors.confirmNewPassword ? 'input-error' : ''}`}
            />
            {errors.confirmNewPassword && <p className="error-text">{errors.confirmNewPassword}</p>}
          </div>
          <div className="flex justify-end gap-3 pt-3 border-t border-neutral-200">
            <button type="button" onClick={() => setPasswordModal(false)} className="btn btn-secondary">
              Cancelar
            </button>
            <button type="submit" disabled={saving} className="btn btn-primary">
              {saving ? <Spinner size="sm" /> : 'Cambiar contrasena'}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
