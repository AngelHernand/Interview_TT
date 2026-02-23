import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import userService from '../services/userService';
import Spinner from '../components/UI/Spinner';
import toast from 'react-hot-toast';
import { ArrowLeft, Pencil } from 'lucide-react';

export default function UserEdit() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [formData, setFormData] = useState({ nombre: '', email: '', rolId: 2 });
  const [errors, setErrors] = useState({});

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const result = await userService.getById(id);
        if (result.success) {
          const u = result.data;
          setFormData({ nombre: u.nombre, email: u.email, rolId: u.rol === 'Admin' ? 1 : 2 });
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

  const validate = () => {
    const newErrors = {};
    if (!formData.nombre?.trim()) newErrors.nombre = 'Requerido';
    if (formData.email && !/\S+@\S+\.\S+/.test(formData.email)) newErrors.email = 'Email invalido';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: name === 'rolId' ? parseInt(value) : value }));
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;

    setSaving(true);
    try {
      const result = await userService.update(id, formData);
      if (result.success) {
        toast.success('Usuario actualizado correctamente');
        navigate('/users');
      }
    } catch (error) {
      const message = error.response?.data?.message || 'Error al actualizar usuario';
      toast.error(message);
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
    <div className="max-w-lg mx-auto">
      <button
        onClick={() => navigate('/users')}
        className="btn btn-ghost btn-sm mb-5 -ml-2 text-neutral-500"
      >
        <ArrowLeft size={14} />
        Usuarios
      </button>

      <div className="card card-accent-top">
        <div className="flex items-center gap-3 mb-5">
          <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-accent-100">
            <Pencil size={17} className="text-accent-600" />
          </div>
          <h1 className="text-base font-bold text-neutral-900">Editar usuario</h1>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="label">Nombre</label>
            <input
              name="nombre"
              value={formData.nombre}
              onChange={handleChange}
              className={`input-field ${errors.nombre ? 'input-error' : ''}`}
            />
            {errors.nombre && <p className="error-text">{errors.nombre}</p>}
          </div>

          <div>
            <label className="label">Email</label>
            <input
              name="email"
              type="email"
              value={formData.email}
              onChange={handleChange}
              className={`input-field ${errors.email ? 'input-error' : ''}`}
            />
            {errors.email && <p className="error-text">{errors.email}</p>}
          </div>

          <div>
            <label className="label">Rol</label>
            <select name="rolId" value={formData.rolId} onChange={handleChange} className="input-field">
              <option value={2}>User</option>
              <option value={1}>Admin</option>
            </select>
          </div>

          <div className="flex justify-end gap-3 pt-3 border-t border-neutral-200 mt-5">
            <button type="button" onClick={() => navigate('/users')} className="btn btn-secondary">
              Cancelar
            </button>
            <button type="submit" disabled={saving} className="btn btn-primary">
              {saving ? <Spinner size="sm" /> : 'Guardar cambios'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
