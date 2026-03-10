# Interview TT - Plataforma de Evaluacion Tecnica

Aplicacion fullstack para gestion de usuarios y evaluacion de conocimientos tecnicos en .NET y C#. Incluye backend API REST, frontend SPA, suite de pruebas automatizadas y orquestacion completa con Docker Compose.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=white)
![Vite](https://img.shields.io/badge/Vite-7-646CFF?logo=vite&logoColor=white)
![TailwindCSS](https://img.shields.io/badge/TailwindCSS-4-06B6D4?logo=tailwindcss&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-101%20tests-5E1F87)
![Vitest](https://img.shields.io/badge/Vitest-31%20tests-6E9F18)

---

## Tabla de Contenidos

- [Caracteristicas](#caracteristicas)
- [Arquitectura](#arquitectura)
- [Tech Stack](#tech-stack)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Inicio Rapido con Docker](#inicio-rapido-con-docker)
- [Configuracion Manual (sin Docker)](#configuracion-manual-sin-docker)
- [Pruebas Automatizadas](#pruebas-automatizadas)
- [Endpoints de la API](#endpoints-de-la-api)
- [Roles y Permisos](#roles-y-permisos)
- [Modelo de Datos](#modelo-de-datos)
- [Licencia](#licencia)

---

## Caracteristicas

### Backend (API REST - ASP.NET Core 8)

- Autenticacion JWT con refresh tokens y rotacion automatica
- Registro e inicio de sesion con hash BCrypt (WorkFactor 11)
- CRUD completo de usuarios con roles (Admin / User)
- Banco de 15 preguntas tecnicas de opcion multiple (.NET / C#)
- Evaluacion automatica de respuestas con calculo de puntaje
- Auditoria de acciones mediante middleware personalizado
- Registro de intentos de login con bloqueo por intentos fallidos
- Validacion de DTOs con FluentValidation
- Logging estructurado con Serilog (consola + archivo rotativo)
- Manejo global de excepciones con respuestas estandarizadas
- Documentacion Swagger/OpenAPI

### Frontend (SPA - React 19)

- Landing page publica con informacion del sistema
- Interfaz con diseno enterprise (paleta indigo + teal)
- Sidebar oscuro con navegacion contextual por rol
- Login y registro con layout dividido y panel decorativo
- Dashboard con banner gradiente y tarjetas estadisticas
- Test de conocimientos con 15 preguntas, barra de progreso y temporizador
- Resultados con score dinamico y codigo de colores (verde/amarillo/rojo)
- Gestion de usuarios: listado paginado, creacion, edicion, detalle
- Perfil de usuario con formulario de cambio de contrasena
- Modales de confirmacion, spinners de carga, notificaciones toast
- Rutas protegidas por autenticacion y rol
- Redireccion inteligente segun estado de sesion

### Infraestructura

- Contenedorizacion completa con Docker Compose (4 servicios)
- Dockerfiles multi-stage con etapas de build, test y runtime
- Inicializacion automatica de base de datos (DDL + datos semilla)
- Proxy inverso Nginx con SPA routing y reenvio a la API
- Health checks en todos los servicios con dependencias ordenadas
- Suite de pruebas ejecutable como contenedor independiente

---

## Arquitectura

```
                    +-------------------+
                    |    Browser        |
                    |  localhost:3000   |
                    +--------+----------+
                             |
                    +--------v----------+
                    |  Nginx (Frontend) |
                    |  React SPA        |
                    |  Proxy /api/ --+  |
                    +--------+------+---+
                             |      |
                             |      |  /api/*
                             |      |
                    +--------v------v---+
                    |  .NET 8 API       |
                    |  localhost:5240    |
                    |  (port 8080 int)  |
                    +--------+----------+
                             |
                    +--------v----------+
                    |  SQL Server 2022  |
                    |  localhost:1433   |
                    +-------------------+
```

Todos los servicios se comunican a traves de una red Docker interna (`app-network`). El frontend sirve archivos estaticos y actua como proxy inverso, reenviando las peticiones `/api/*` al backend.

---

## Tech Stack

| Capa | Tecnologia |
|------|-----------|
| **Backend** | ASP.NET Core 8, Entity Framework Core 8 (Database First) |
| **Autenticacion** | JWT Bearer + Refresh Tokens, BCrypt.Net-Next |
| **Validacion** | FluentValidation |
| **Logging** | Serilog (consola + archivo) |
| **Base de datos** | SQL Server 2022 |
| **Frontend** | React 19, Vite 7, TailwindCSS v4 |
| **Routing** | React Router DOM 7 |
| **Cliente HTTP** | Axios |
| **Iconos** | Lucide React |
| **Notificaciones** | React Hot Toast |
| **Tests Backend** | xUnit, Moq, FluentAssertions |
| **Tests Frontend** | Vitest, React Testing Library, jsdom |
| **Contenedores** | Docker, Docker Compose |
| **Servidor Web** | Nginx Alpine (proxy inverso + SPA) |

---

## Estructura del Proyecto

```
Interview_Base/
|
|-- Interview_Base/                # API Backend (.NET 8)
|   |-- Controllers/               #   Auth, Users, Questions, Audit
|   |-- DTOs/                      #   Data Transfer Objects por dominio
|   |-- Models/                    #   Entidades EF Core (Usuario, Rol, etc.)
|   |-- Data/                      #   DbContext + banco de preguntas
|   |-- Services/                  #   Logica de negocio (+ interfaces)
|   |-- Repositories/              #   Acceso a datos (+ interfaces)
|   |-- Middleware/                #   Auditoria + excepciones globales
|   |-- Validators/                #   FluentValidation (Login, Register, etc.)
|   |-- Helpers/                   #   JWT generation + Password hashing
|   |-- Extensions/                #   Configuracion de DI (ServiceExtensions)
|   |-- Dockerfile                 #   Multi-stage: build > test > publish > runtime
|   |-- Program.cs                 #   Entry point + pipeline de configuracion
|   `-- appsettings.json           #   Configuracion (conexion, JWT, seguridad)
|
|-- Interview_Base.Tests/          # Pruebas Backend (xUnit)
|   |-- Controllers/               #   QuestionsControllerTests (10 tests)
|   |-- Services/                  #   AuthServiceTests (12), UserServiceTests (16)
|   |-- Helpers/                   #   JwtHelperTests (9), PasswordHelperTests (7)
|   |-- Validators/                #   ValidatorTests (14)
|   `-- Data/                      #   QuestionBankTests (13)
|
|-- users-web/                     # Frontend React
|   |-- src/
|   |   |-- components/            #   Layout, Auth guards, UI compartidos
|   |   |-- pages/                 #   Landing, Login, Dashboard, Users, Test, Profile
|   |   |-- services/              #   Clientes API (authService, userService, etc.)
|   |   |-- context/               #   AuthContext (estado JWT + refresh)
|   |   |-- hooks/                 #   useAuth (hook de autenticacion)
|   |   |-- test/                  #   Vitest: auth, user, question services + Login
|   |   |-- index.css              #   Design system tokens
|   |   `-- App.jsx                #   Router + definicion de rutas
|   |-- Dockerfile                 #   Multi-stage: build > test > publish (nginx)
|   |-- nginx.conf                 #   SPA routing + proxy /api/ al backend
|   `-- vite.config.js             #   Configuracion Vite + proxy dev
|
|-- docker/                        # Scripts de inicializacion Docker
|   |-- init-db.sql                #   DDL completo + datos semilla
|   `-- init-db.sh                 #   Script de espera y ejecucion
|
|-- docker-compose.yml             # Orquestacion de servicios
|-- .env.example                   # Variables de entorno (plantilla)
|-- .dockerignore                  # Exclusiones para contexto Docker
`-- Interview_Base.sln             # Solucion .NET
```

---

## Inicio Rapido con Docker

> **Requisitos:** [Docker Desktop](https://www.docker.com/products/docker-desktop/) (incluye Docker Compose).

### 1. Clonar el repositorio

```bash
git clone https://github.com/AngelHernand/Interview_TT.git
cd Interview_TT
```

### 2. Configurar variables de entorno

```bash
cp .env.example .env
```

Editar `.env` si se desea cambiar las credenciales por defecto:

```env
SA_PASSWORD=DockerPass123!
JWT_SECRET=Docker_Clave_Secreta_MuySegura_32chars!!
```

### 3. Levantar toda la aplicacion

```bash
docker compose up --build -d
```

Este comando construye las imagenes y levanta los siguientes servicios:

| Servicio | Contenedor | Puerto | Descripcion |
|----------|-----------|--------|-------------|
| SQL Server | `interview_sqlserver` | `1433` | Base de datos |
| Init DB | `interview_db_init` | -- | Crea tablas y datos semilla, luego se detiene |
| Backend | `interview_backend` | `5240` | API REST .NET 8 |
| Frontend | `interview_frontend` | `3000` | React SPA + Nginx |

### 4. Acceder a la aplicacion

| Recurso | URL |
|---------|-----|
| **Aplicacion web** | http://localhost:3000 |
| **Swagger API** | http://localhost:5240/swagger |

**Credenciales del administrador por defecto:**

| Campo | Valor |
|-------|-------|
| Email | `admin@sistema.com` |
| Contrasena | `Admin123!` |

### 5. Detener la aplicacion

```bash
# Detener contenedores (preserva datos de la BD)
docker compose down

# Detener y eliminar volumen de datos
docker compose down -v
```

---

## Configuracion Manual (sin Docker)

### Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/sql-server) (local o remoto)

### 1. Base de Datos

Ejecutar el script `docker/init-db.sql` en SQL Server Management Studio o `sqlcmd` para crear la base de datos `UsersDB` con todas las tablas, la vista y los datos semilla.

Tablas del esquema:

| Tabla | Descripcion |
|-------|-------------|
| `Roles` | Catalogo de roles (Admin, User) |
| `Usuarios` | Usuarios del sistema con referencia a rol |
| `RefreshTokens` | Tokens de refresco JWT |
| `LoginAttempts` | Registro de intentos de inicio de sesion |
| `AuditLogs` | Bitacora de auditoria de acciones |
| `vw_UsuariosActivos` | Vista de usuarios activos con nombre de rol |

### 2. Backend

Configurar la cadena de conexion en `Interview_Base/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=UsersDB;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True;Encrypt=False;"
  },
  "JwtSettings": {
    "Secret": "TU_CLAVE_SECRETA_DE_AL_MENOS_32_CARACTERES",
    "Issuer": "UsersAPI",
    "Audience": "UsersAPI",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "Security": {
    "MaxLoginAttempts": 5,
    "LockoutDurationMinutes": 30
  }
}
```

Ejecutar:

```bash
cd Interview_Base
dotnet run
```

La API estara disponible en `http://localhost:5240`.

### 3. Frontend

```bash
cd users-web
npm install
npm run dev
```

El frontend estara disponible en `http://localhost:3000`. El proxy de Vite redirige `/api` al backend en puerto 5240 durante el desarrollo.

---

## Pruebas Automatizadas

El proyecto cuenta con **132 pruebas automatizadas** distribuidas en dos suites.

### Backend -- xUnit (101 pruebas)

| Archivo | Cobertura | Pruebas |
|---------|-----------|---------|
| `PasswordHelperTests.cs` | Hashing, verificacion, work factor | 7 |
| `JwtHelperTests.cs` | Generacion de tokens, claims, expiracion | 9 |
| `QuestionBankTests.cs` | Banco de preguntas, aleatoriedad, evaluacion | 13 |
| `ValidatorTests.cs` | Login, registro, password, creacion de usuario | 14 |
| `AuthServiceTests.cs` | Login, registro, refresh token, cambio de password | 12 |
| `UserServiceTests.cs` | CRUD de usuarios, paginacion, validaciones | 16 |
| `QuestionsControllerTests.cs` | Endpoints GET y POST del controlador | 10 |

Ejecutar las pruebas de backend:

```bash
cd Interview_Base.Tests
dotnet test
```

### Frontend -- Vitest (31 pruebas)

| Archivo | Cobertura | Pruebas |
|---------|-----------|---------|
| `authService.test.js` | Login, registro, refresh, cambio de password | 12 |
| `questionService.test.js` | Obtener preguntas, evaluar respuestas | 4 |
| `userService.test.js` | CRUD usuarios, paginacion, errores | 8 |
| `Login.test.jsx` | Renderizado, validacion, submit, navegacion | 7 |

Ejecutar las pruebas de frontend:

```bash
cd users-web
npx vitest run
```

### Ejecutar pruebas en Docker

```bash
docker compose --profile test up --build
```

Esto levanta dos contenedores adicionales (`interview_tests_backend` y `interview_tests_frontend`) que ejecutan las suites y reportan los resultados en la salida estandar.

---

## Endpoints de la API

### Autenticacion

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | `/api/auth/register` | Registrar nuevo usuario |
| POST | `/api/auth/login` | Iniciar sesion (devuelve JWT + refresh token) |
| POST | `/api/auth/refresh-token` | Renovar JWT con refresh token |
| PUT | `/api/auth/change-password` | Cambiar contrasena (requiere autenticacion) |

### Usuarios (requiere rol Admin)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | `/api/users` | Listar usuarios (paginado, query params: page, pageSize) |
| GET | `/api/users/{id}` | Obtener detalle de un usuario |
| POST | `/api/users` | Crear usuario |
| PUT | `/api/users/{id}` | Actualizar usuario |
| DELETE | `/api/users/{id}` | Eliminar usuario (soft delete) |

### Preguntas (requiere autenticacion)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | `/api/questions` | Obtener 15 preguntas aleatorias |
| POST | `/api/questions/evaluate` | Evaluar respuestas y obtener puntaje |

### Auditoria (requiere rol Admin)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | `/api/audit` | Obtener registros de auditoria |

Todas las respuestas siguen el formato estandar:

```json
{
  "success": true,
  "message": "Operacion exitosa",
  "data": { },
  "errors": []
}
```

---

## Roles y Permisos

| Rol | Acceso |
|-----|--------|
| **Admin** | Dashboard completo, gestion de usuarios (CRUD), test de conocimientos, perfil, auditoria |
| **User** | Dashboard limitado, test de conocimientos, perfil personal |

Los usuarios no autenticados ven la landing page publica y pueden registrarse o iniciar sesion.

---

## Modelo de Datos

```
+-------------+       +---------------+       +------------------+
|   Roles     |       |   Usuarios    |       |  RefreshTokens   |
|-------------|       |---------------|       |------------------|
| Id (PK)     |<------| Id (PK, GUID) |------>| Id (PK)          |
| Nombre      |       | Nombre        |       | Token            |
| Descripcion |       | Email (UQ)    |       | UsuarioId (FK)   |
+-------------+       | PasswordHash  |       | FechaExpiracion  |
                      | RolId (FK)    |       | FechaCreacion    |
                      | Activo        |       | Usado            |
                      | FechaCreacion |       +------------------+
                      +-------+-------+
                              |
              +---------------+---------------+
              |                               |
    +---------v--------+           +----------v---------+
    |  LoginAttempts   |           |    AuditLogs       |
    |------------------|           |--------------------|
    | Id (PK)          |           | Id (PK)            |
    | Email            |           | UsuarioId          |
    | Exitoso          |           | Accion             |
    | DireccionIP      |           | Entidad            |
    | FechaIntento     |           | EntidadId          |
    +------------------+           | DetallesJson       |
                                   | DireccionIP        |
                                   | FechaCreacion      |
                                   +--------------------+
```

---

## Licencia

Este proyecto es de uso privado / evaluacion tecnica.
