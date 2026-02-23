import { useState } from 'react';
import { Link, Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import Spinner from '../components/UI/Spinner';
import { Hexagon, ArrowRight } from 'lucide-react';

export default function Register() {
  const { register, isAuthenticated, isAdmin } = useAuth();
  const [formData, setFormData] = useState({
    nombre: '',
    email: '',
    password: '',
    confirmPassword: '',
  });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [apiError, setApiError] = useState('');

  if (isAuthenticated) {
    return <Navigate to={isAdmin ? '/dashboard' : '/test'} replace />;
  }

  const validate = () => {
    const newErrors = {};
    if (!formData.nombre.trim()) newErrors.nombre = 'El nombre es requerido';
    if (!formData.email) {
      newErrors.email = 'El email es requerido';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email invalido';
    }
    if (!formData.password) {
      newErrors.password = 'La contrasena es requerida';
    } else if (formData.password.length < 8) {
      newErrors.password = 'Minimo 8 caracteres';
    } else if (!/[A-Z]/.test(formData.password)) {
      newErrors.password = 'Debe contener al menos una mayuscula';
    } else if (!/[0-9]/.test(formData.password)) {
      newErrors.password = 'Debe contener al menos un numero';
    } else if (!/[^a-zA-Z0-9]/.test(formData.password)) {
      newErrors.password = 'Debe contener al menos un caracter especial';
    }
    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Las contrasenas no coinciden';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }));
    setApiError('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;

    setLoading(true);
    setApiError('');
    const result = await register(
      formData.nombre,
      formData.email,
      formData.password,
      formData.confirmPassword
    );
    if (!result.success) {
      setApiError(result.errors?.length ? result.errors.join(', ') : result.message);
    }
    setLoading(false);
  };

  return (
    <div className="min-h-screen flex">
      {/* Left panel — decorative */}
      <div className="hidden lg:flex lg:w-[420px] gradient-hero relative overflow-hidden items-center justify-center">
        <div className="absolute -top-20 -left-20 w-72 h-72 rounded-full bg-white/5" />
        <div className="absolute -bottom-32 -right-16 w-96 h-96 rounded-full bg-white/5" />
        <div className="absolute top-1/3 right-10 w-40 h-40 rounded-full bg-white/5" />

        <div className="relative z-10 text-center px-10">
          <div className="inline-flex items-center justify-center w-16 h-16 rounded-2xl bg-white/10 backdrop-blur mb-6">
            <Hexagon size={32} className="text-white" />
          </div>
          <h2 className="text-2xl font-bold text-white mb-3">Unete a la plataforma</h2>
          <p className="text-primary-200 text-sm leading-relaxed">
            Crea tu cuenta para acceder a las evaluaciones y gestionar tu perfil.
          </p>
        </div>
      </div>

      {/* Right panel — form */}
      <div className="flex-1 flex items-center justify-center bg-neutral-50 px-6">
        <div className="w-full max-w-[400px] animate-fade-in">
          {/* Mobile logo */}
          <div className="text-center mb-8 lg:text-left">
            <div className="lg:hidden inline-flex items-center justify-center w-12 h-12 rounded-xl gradient-accent mb-4">
              <Hexagon size={22} className="text-white" />
            </div>
            <h1 className="text-2xl font-bold text-neutral-900">Crear cuenta</h1>
            <p className="mt-1 text-sm text-neutral-500">Completa los campos para registrarte</p>
          </div>

          {/* Card */}
          <div className="card">
            {apiError && (
              <div className="mb-5 px-4 py-3 rounded-lg text-sm text-error bg-error-light border border-red-200">
                {apiError}
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label htmlFor="nombre" className="label">Nombre</label>
                <input
                  id="nombre"
                  name="nombre"
                  type="text"
                  value={formData.nombre}
                  onChange={handleChange}
                  className={`input-field ${errors.nombre ? 'input-error' : ''}`}
                  placeholder="Juan Perez"
                />
                {errors.nombre && <p className="error-text">{errors.nombre}</p>}
              </div>

              <div>
                <label htmlFor="email" className="label">Email</label>
                <input
                  id="email"
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
                <label htmlFor="password" className="label">Contrasena</label>
                <input
                  id="password"
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
                <label htmlFor="confirmPassword" className="label">Confirmar contrasena</label>
                <input
                  id="confirmPassword"
                  name="confirmPassword"
                  type="password"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  className={`input-field ${errors.confirmPassword ? 'input-error' : ''}`}
                  placeholder="********"
                />
                {errors.confirmPassword && <p className="error-text">{errors.confirmPassword}</p>}
              </div>

              <button type="submit" disabled={loading} className="btn btn-primary w-full mt-2">
                {loading ? <Spinner size="sm" /> : (<>Crear cuenta <ArrowRight size={16} /></>)}
              </button>
            </form>
          </div>

          <p className="mt-6 text-center lg:text-left text-sm text-neutral-500">
            Ya tienes cuenta?{' '}
            <Link to="/login" className="font-semibold text-primary-600 hover:text-primary-700 transition-colors">
              Inicia sesion
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}
