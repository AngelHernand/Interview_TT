# Users Web - Aplicación Frontend React

Aplicación web desarrollada con **React + Vite** y **TailwindCSS** para consumir la API de gestión de usuarios.

## Tecnologías

- **React 18** - Librería de UI
- **Vite** - Build tool y dev server
- **TailwindCSS 4** - Framework CSS utilitario
- **React Router DOM** - Enrutamiento SPA
- **Axios** - Cliente HTTP
- **React Hot Toast** - Notificaciones

## Requisitos Previos

- Node.js 18+
- npm o yarn
- API Backend corriendo en `http://localhost:5000`

## Instalación

```bash
cd users-web
npm install
```

## Ejecución

```bash
npm run dev
```

La aplicación estará disponible en [http://localhost:3000](http://localhost:3000)

## Estructura del Proyecto

```
src/
├── components/
│   ├── Auth/ProtectedRoute.jsx
│   ├── Layout/MainLayout.jsx, Navbar.jsx
│   └── UI/Modal.jsx, Pagination.jsx, Spinner.jsx
├── context/AuthContext.jsx
├── hooks/useAuth.js
├── pages/
│   ├── Login.jsx, Register.jsx
│   ├── Dashboard.jsx, Profile.jsx
│   └── Users.jsx, UserCreate.jsx, UserEdit.jsx, UserDetail.jsx
├── services/api.js, authService.js, userService.js
├── App.jsx
└── index.css
```

## Rutas

| Ruta | Acceso | Descripción |
|------|--------|-------------|
| `/login` | Público | Inicio de sesión |
| `/register` | Público | Registro |
| `/dashboard` | Autenticado | Panel principal |
| `/users` | Autenticado | Lista de usuarios |
| `/users/:id` | Autenticado | Detalle de usuario |
| `/users/:id/edit` | Autenticado | Editar usuario |
| `/users/new` | Admin | Crear usuario |
| `/profile` | Autenticado | Mi perfil |

## Características

- Autenticación JWT con refresh token automático
- Protección de rutas (públicas, autenticadas, admin)
- CRUD completo de usuarios
- Bloqueo/desbloqueo de usuarios (admin)
- Cambio de contraseña
- Notificaciones toast
- Diseño responsive
- Paginación en listados
