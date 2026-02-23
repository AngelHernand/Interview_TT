import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import userService from '../services/userService';
import Spinner from '../components/UI/Spinner';
import toast from 'react-hot-toast';
import { ArrowLeft, UserPlus } from 'lucide-react';

export default function UserCreate() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    nombre: '',
    email: '',
    password: '',
    rolId: 2,
  });
  const [errors, setErrors] = useState({});

  const validate = () => {
    const newErrors = {};
    if (!formData.nombre.trim()) newErrors.nombre = 'Requerido';
    if (!formData.email) {
      newErrors.email = 'Requerido';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email invalido';
    }
    if (!formData.password) {
      newErrors.password = 'Requerido';
    } else if (formData.password.length < 8) {
      newErrors.password = 'Minimo 8 caracteres';
    }
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

    setLoading(true);
    try {
      const result = await userService.create(formData);
      if (result.success) {
        toast.success('Usuario creado correctamente');
        navigate('/users');
      }
    } catch (error) {
      const message = error.response?.data?.message || 'Error al crear usuario';
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-lg mx-auto">
      {/* Back link */}
      <button
        onClick={() => navigate('/users')}
        className="btn btn-ghost btn-sm mb-5 -ml-2 text-neutral-500"
      >
        <ArrowLeft size={14} />
        Usuarios
      </button>

      <div className="card card-accent-top">
        <div className="flex items-center gap-3 mb-5">
          <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-primary-100">
            <UserPlus size={17} className="text-primary-600" />
          </div>
          <h1 className="text-base font-bold text-neutral-900">Crear usuario</h1>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="label">Nombre</label>
            <input
              name="nombre"
              value={formData.nombre}
              onChange={handleChange}
              className={`input-field ${errors.nombre ? 'input-error' : ''}`}
              placeholder="Juan Perez"
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
              placeholder="correo@ejemplo.com"
            />
            {errors.email && <p className="error-text">{errors.email}</p>}
          </div>

          <div>
            <label className="label">Contrasena</label>
            <input
              name="password"
              type="password"
              value={formData.password}
              onChange={handleChange}
              className={`input-field ${errors.password ? 'input-error' : ''}`}
              placeholder="********"
            />
            {errors.password && <p className="error-text">{errors.password}</p>}
            <p className="helper-text">
              Minimo 8 caracteres, 1 mayuscula, 1 numero, 1 caracter especial
            </p>
          </div>

          <div>
            <label className="label">Rol</label>
            <select
              name="rolId"
              value={formData.rolId}
              onChange={handleChange}
              className="input-field"
            >
              <option value={2}>User</option>
              <option value={1}>Admin</option>
            </select>
          </div>

          <div className="flex justify-end gap-3 pt-3 border-t border-neutral-200 mt-5">
            <button type="button" onClick={() => navigate('/users')} className="btn btn-secondary">
              Cancelar
            </button>
            <button type="submit" disabled={loading} className="btn btn-primary">
              {loading ? <Spinner size="sm" /> : 'Crear usuario'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
