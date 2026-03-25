import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/Auth/ProtectedRoute';
import MainLayout from './components/Layout/MainLayout';
import SmartRedirect from './components/Auth/SmartRedirect';

// Pages
import Landing from './pages/Landing';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Users from './pages/Users';
import UserCreate from './pages/UserCreate';
import UserEdit from './pages/UserEdit';
import UserDetail from './pages/UserDetail';
import Profile from './pages/Profile';

// Test Pages
import TestHome from './pages/TestHome';
import TestQuestion from './pages/TestQuestion';
import TestStats from './pages/TestStats';

// Interview Pages
import InterviewSetup from './pages/InterviewSetup';
import InterviewChat from './pages/InterviewChat';
import InterviewResults from './pages/InterviewResults';
import InterviewHistory from './pages/InterviewHistory';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          {/* Public Routes */}
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />

          {/* Protected Routes — Test (todos los usuarios autenticados) */}
          <Route element={<ProtectedRoute />}>
            <Route element={<MainLayout />}>
              <Route path="/test" element={<TestHome />} />
              <Route path="/test/questions" element={<TestQuestion />} />
              <Route path="/test/stats" element={<TestStats />} />
              <Route path="/interview" element={<InterviewHistory />} />
              <Route path="/interview/setup" element={<InterviewSetup />} />
              <Route path="/interview/:sessionId" element={<InterviewChat />} />
              <Route path="/interview/:sessionId/results" element={<InterviewResults />} />
              <Route path="/profile" element={<Profile />} />

              {/* Admin Only Routes */}
              <Route element={<ProtectedRoute adminOnly />}>
                <Route path="/dashboard" element={<Dashboard />} />
                <Route path="/users" element={<Users />} />
                <Route path="/users/new" element={<UserCreate />} />
                <Route path="/users/:id" element={<UserDetail />} />
                <Route path="/users/:id/edit" element={<UserEdit />} />
              </Route>
            </Route>
          </Route>

          {/* Landing page (public) — redirige si ya está autenticado */}
          <Route path="/" element={<Landing />} />
          {/* Catch-all: redirige según rol o a landing */}
          <Route path="*" element={<SmartRedirect />} />
        </Routes>
      
      {/* Toast Notifications */}
      <Toaster
        position="top-right"
        toastOptions={{
          duration: 3000,
          style: {
            background: '#1F2933',
            color: '#F7F9FA',
            fontSize: '13px',
            borderRadius: '6px',
            padding: '10px 14px',
          },
          success: {
            style: {
              background: '#1F2933',
            },
            iconTheme: {
              primary: '#10B981',
              secondary: '#F7F9FA',
            },
          },
          error: {
            style: {
              background: '#1F2933',
            },
            iconTheme: {
              primary: '#EF4444',
              secondary: '#F7F9FA',
            },
          },
        }}
      />
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
