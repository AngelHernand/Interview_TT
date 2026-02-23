import { useState } from 'react';
import { Link, Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import Spinner from '../components/UI/Spinner';
import { Hexagon, ArrowRight } from 'lucide-react';

export default function Login() {
  const { login, isAuthenticated, isAdmin } = useAuth();
  const [formData, setFormData] = useState({ email: '', password: '' });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [apiError, setApiError] = useState('');

  if (isAuthenticated) {
    return <Navigate to={isAdmin ? '/dashboard' : '/test'} replace />;
  }

  const validate = () => {
    const newErrors = {};
    if (!formData.email) {
      newErrors.email = 'El email es requerido';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email invalido';
    }
    if (!formData.password) {
      newErrors.password = 'La contrasena es requerida';
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
    const result = await login(formData.email, formData.password);
    if (!result.success) setApiError(result.message);
    setLoading(false);
  };

  return (
    <div className="min-h-screen flex">
      {/* Left panel — decorative */}
      <div className="hidden lg:flex lg:w-[420px] gradient-hero relative overflow-hidden items-center justify-center">
        {/* Decorative circles */}
        <div className="absolute -top-20 -left-20 w-72 h-72 rounded-full bg-white/5" />
        <div className="absolute -bottom-32 -right-16 w-96 h-96 rounded-full bg-white/5" />
        <div className="absolute top-1/3 right-10 w-40 h-40 rounded-full bg-white/5" />

        <div className="relative z-10 text-center px-10">
          <div className="inline-flex items-center justify-center w-16 h-16 rounded-2xl bg-white/10 backdrop-blur mb-6">
            <Hexagon size={32} className="text-white" />
          </div>
          <h2 className="text-2xl font-bold text-white mb-3">UsersApp</h2>
          <p className="text-primary-200 text-sm leading-relaxed">
            Plataforma de gestion de usuarios y evaluaciones tecnicas para equipos.
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
            <h1 className="text-2xl font-bold text-neutral-900">Iniciar sesion</h1>
            <p className="mt-1 text-sm text-neutral-500">Ingresa tus credenciales para continuar</p>
          </div>

          {/* Card */}
          <div className="card">
            {apiError && (
              <div className="mb-5 px-4 py-3 rounded-lg text-sm text-error bg-error-light border border-red-200">
                {apiError}
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-5">
              <div>
                <label htmlFor="email" className="label">Email</label>
                <input
                  id="email"
                  name="email"
                  type="email"
                  autoComplete="email"
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
                  autoComplete="current-password"
                  value={formData.password}
                  onChange={handleChange}
                  className={`input-field ${errors.password ? 'input-error' : ''}`}
                  placeholder="********"
                />
                {errors.password && <p className="error-text">{errors.password}</p>}
              </div>

              <button type="submit" disabled={loading} className="btn btn-primary w-full">
                {loading ? <Spinner size="sm" /> : (<>Iniciar sesion <ArrowRight size={16} /></>)}
              </button>
            </form>
          </div>

          <p className="mt-6 text-center lg:text-left text-sm text-neutral-500">
            No tienes cuenta?{' '}
            <Link to="/register" className="font-semibold text-primary-600 hover:text-primary-700 transition-colors">
              Registrate
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}
