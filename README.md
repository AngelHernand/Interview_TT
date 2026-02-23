# Interview TT - Plataforma de Evaluación Técnica

Aplicación fullstack para gestión de usuarios y evaluación de conocimientos técnicos, construida con **ASP.NET Core 8** y **React + Vite**.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=white)
![Vite](https://img.shields.io/badge/Vite-7-646CFF?logo=vite&logoColor=white)
![TailwindCSS](https://img.shields.io/badge/TailwindCSS-4-06B6D4?logo=tailwindcss&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?logo=microsoftsqlserver&logoColor=white)

---

## Características

### Backend (API REST)
- Autenticación JWT con refresh tokens
- Registro e inicio de sesión con hash BCrypt
- CRUD completo de usuarios con roles (Admin / User)
- Banco de 15 preguntas técnicas con evaluación automática
- Auditoría de acciones (middleware personalizado)
- Validación con FluentValidation
- Logging estructurado con Serilog
- Manejo global de excepciones

### Frontend (SPA)
- Interfaz moderna con diseño enterprise (indigo + teal)
- Sidebar oscuro con gradientes
- Login/Register con layout dividido y panel decorativo
- Dashboard con banner gradiente y tarjetas estadísticas
- Test de conocimientos con barra de progreso animada
- Resultados con tarjeta de score con gradiente dinámico (verde/amarillo/rojo)
- Gestión de usuarios (tabla, crear, editar, detalle)
- Perfil de usuario con cambio de contraseña
- Paginación, modales de confirmación, spinners
- Rutas protegidas por rol
- Notificaciones toast

---

## Tech Stack

| Capa | Tecnología |
|------|-----------|
| **Backend** | ASP.NET Core 8, EF Core 8 (Database First) |
| **Auth** | JWT Bearer + Refresh Tokens, BCrypt |
| **Validación** | FluentValidation |
| **Logging** | Serilog (consola + archivo) |
| **Base de datos** | SQL Server |
| **Frontend** | React 19, Vite 7, TailwindCSS v4 |
| **Routing** | React Router DOM 7 |
| **HTTP Client** | Axios |
| **Iconos** | Lucide React |
| **Notificaciones** | React Hot Toast |

---

## Estructura del Proyecto

```
Interview_Base/
├── Interview_Base/              # API Backend (.NET 8)
│   ├── Controllers/             # Auth, Users, Questions, Audit
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Models/                  # Entidades EF Core
│   ├── Data/                    # DbContext + banco de preguntas
│   ├── Services/                # Lógica de negocio
│   ├── Repositories/            # Acceso a datos
│   ├── Middleware/               # Auditoría + manejo de excepciones
│   ├── Validators/              # FluentValidation
│   ├── Helpers/                 # JWT + Password helpers
│   ├── Extensions/              # Service extensions (DI)
│   └── Program.cs               # Entry point + configuración
│
├── users-web/                   # Frontend React
│   ├── src/
│   │   ├── components/          # Layout, Auth guards, UI
│   │   ├── pages/               # Dashboard, Login, Users, Test, Profile
│   │   ├── services/            # API clients (axios)
│   │   ├── context/             # AuthContext (JWT state)
│   │   ├── hooks/               # Custom hooks
│   │   ├── index.css            # Design system + tokens
│   │   └── App.jsx              # Router + rutas
│   └── vite.config.js
│
└── Interview_Base.sln           # Solución .NET
```

---

## Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/sql-server) (local o remoto)

---

## Configuración

### 1. Base de Datos

La API usa **EF Core Database First**. Asegúrate de tener la base de datos `UsersDB` creada en SQL Server con las tablas necesarias:

- `Usuarios` — usuarios del sistema
- `Roles` — roles (Admin, User)
- `RefreshTokens` — tokens de refresco
- `LoginAttempts` — intentos de login
- `AuditLogs` — registro de auditoría
- `vwUsuariosActivos` — vista de usuarios activos

### 2. Backend

Configura la cadena de conexión en `Interview_Base/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=UsersDB;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True;Encrypt=False;"
  },
  "JwtSettings": {
    "SecretKey": "TU_CLAVE_SECRETA_DE_AL_MENOS_32_CARACTERES",
    "Issuer": "InterviewBase",
    "Audience": "InterviewBaseUsers",
    "ExpirationMinutes": 60
  }
}
```

Ejecutar el backend:

```bash
cd Interview_Base
dotnet run
```

La API estará disponible en `http://localhost:5240`.

### 3. Frontend

```bash
cd users-web
npm install
npm run dev
```

El frontend estará disponible en `http://localhost:3000`.

---

## Endpoints Principales

### Auth
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/auth/register` | Registrar usuario |
| POST | `/api/auth/login` | Iniciar sesión |
| POST | `/api/auth/refresh-token` | Refrescar JWT |
| PUT | `/api/auth/change-password` | Cambiar contraseña |

### Users (Admin)
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/users` | Listar usuarios (paginado) |
| GET | `/api/users/{id}` | Obtener usuario |
| POST | `/api/users` | Crear usuario |
| PUT | `/api/users/{id}` | Actualizar usuario |
| DELETE | `/api/users/{id}` | Eliminar usuario |

### Questions
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/questions` | Obtener 15 preguntas aleatorias |
| POST | `/api/questions/evaluate` | Evaluar respuestas |

### Audit (Admin)
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/audit` | Obtener logs de auditoría |

---

## Roles

| Rol | Acceso |
|-----|--------|
| **Admin** | Dashboard, gestión de usuarios (CRUD), test, perfil |
| **User** | Dashboard (limitado), test de conocimientos, perfil |



---

## Licencia

Este proyecto es de uso privado / evaluación técnica.
