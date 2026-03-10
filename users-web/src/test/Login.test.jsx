import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import Login from '../pages/Login';

// Mock AuthContext
const mockLogin = vi.fn();
const mockAuthContext = {
  user: null,
  loading: false,
  login: mockLogin,
  register: vi.fn(),
  logout: vi.fn(),
  updateUser: vi.fn(),
  isAuthenticated: false,
  isAdmin: false,
};

vi.mock('../context/AuthContext', () => ({
  useAuth: () => mockAuthContext,
  AuthProvider: ({ children }) => children,
  default: { Provider: ({ children }) => children },
}));

// Mock lucide-react icons
vi.mock('lucide-react', () => ({
  Hexagon: () => <span data-testid="icon-hexagon" />,
  ArrowRight: () => <span data-testid="icon-arrow" />,
}));

// Mock Spinner
vi.mock('../components/UI/Spinner', () => ({
  default: () => <span data-testid="spinner" />,
}));

function renderLogin() {
  return render(
    <MemoryRouter>
      <Login />
    </MemoryRouter>
  );
}

describe('Login Page', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockAuthContext.isAuthenticated = false;
    mockAuthContext.isAdmin = false;
  });

  it('debe renderizar el formulario de login', () => {
    renderLogin();

    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/contrasena/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /iniciar sesion/i })).toBeInTheDocument();
  });

  it('debe mostrar enlace a registro', () => {
    renderLogin();

    expect(screen.getByText(/registrate/i)).toBeInTheDocument();
  });

  it('debe mostrar error si email está vacío al enviar', async () => {
    renderLogin();

    fireEvent.click(screen.getByRole('button', { name: /iniciar sesion/i }));

    await waitFor(() => {
      expect(screen.getByText(/email es requerido/i)).toBeInTheDocument();
    });
    expect(mockLogin).not.toHaveBeenCalled();
  });

  it('debe mostrar error si email es inválido', async () => {
    renderLogin();

    const emailInput = screen.getByLabelText(/email/i);
    const passwordInput = screen.getByLabelText(/contrasena/i);

    // Use fireEvent to bypass HTML5 email validation in jsdom
    fireEvent.change(emailInput, { target: { value: 'invalido', name: 'email' } });
    fireEvent.change(passwordInput, { target: { value: 'Pass123!', name: 'password' } });

    // Submit the form directly
    const form = emailInput.closest('form');
    fireEvent.submit(form);

    await waitFor(() => {
      expect(screen.getByText('Email invalido')).toBeInTheDocument();
    });
  });

  it('debe mostrar error si password está vacía', async () => {
    renderLogin();

    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'test@test.com' } });
    fireEvent.click(screen.getByRole('button', { name: /iniciar sesion/i }));

    await waitFor(() => {
      expect(screen.getByText(/contrasena es requerida/i)).toBeInTheDocument();
    });
  });

  it('debe llamar a login con datos válidos', async () => {
    mockLogin.mockResolvedValue({ success: true });
    renderLogin();

    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'test@test.com' } });
    fireEvent.change(screen.getByLabelText(/contrasena/i), { target: { value: 'Pass123!' } });
    fireEvent.click(screen.getByRole('button', { name: /iniciar sesion/i }));

    await waitFor(() => {
      expect(mockLogin).toHaveBeenCalledWith('test@test.com', 'Pass123!');
    });
  });

  it('debe mostrar error de API tras login fallido', async () => {
    mockLogin.mockResolvedValue({ success: false, message: 'Credenciales inválidas' });
    renderLogin();

    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'bad@test.com' } });
    fireEvent.change(screen.getByLabelText(/contrasena/i), { target: { value: 'Wrong1!' } });
    fireEvent.click(screen.getByRole('button', { name: /iniciar sesion/i }));

    await waitFor(() => {
      expect(screen.getByText(/credenciales inválidas/i)).toBeInTheDocument();
    });
  });
});
