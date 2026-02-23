# UsersAPI — ASP.NET Core 8 Web API

API RESTful para gestión de usuarios con autenticación JWT, arquitectura limpia (Repository + Service Layer) y Entity Framework Core Database First.

---

## Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (local o remoto)
- [Postman](https://www.postman.com/) o cURL (para pruebas)

---

## 1. Restaurar la Base de Datos

Ejecuta el script SQL completo en SQL Server Management Studio (SSMS) para crear la base de datos `UsersDB` con las tablas:

```
Roles, Usuarios, RefreshTokens, AuditLogs, LoginAttempts
Vista: vw_UsuariosActivos
SP: sp_BloquearUsuarioPorIntentos
```

> **Nota:** Asegúrate de insertar los roles iniciales (`Admin`, `User`) y el usuario administrador de prueba.

### Script de datos iniciales (ejemplo)

```sql
-- Roles
INSERT INTO Roles (Nombre, Descripcion) VALUES ('Admin', 'Administrador del sistema');
INSERT INTO Roles (Nombre, Descripcion) VALUES ('User', 'Usuario estándar');

-- Usuario admin de prueba (password: Admin123!)
-- Hash BCrypt generado con workFactor=11
INSERT INTO Usuarios (UsuarioId, Nombre, Apellido, Email, PasswordHash, RolId, Activo, Bloqueado)
VALUES (
    NEWID(),
    'Admin',
    'Sistema',
    'admin@sistema.com',
    '$2a$11$HASH_GENERADO_POR_LA_APP',
    1,
    1,
    0
);
```

> **Recomendación:** Usa el endpoint `POST /api/auth/register` para crear usuarios, o registra el admin via la API primero.

---

## 2. Configuración

Edita `appsettings.json` con tu cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UsersDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## 3. Ejecutar el Proyecto

```bash
cd Interview_Base
dotnet restore
dotnet build
dotnet run
```

La API se inicia en:
- `https://localhost:5001`  
- `http://localhost:5000`

Swagger UI disponible en: `https://localhost:5001/swagger`

---

## 4. Scaffold del DbContext (referencia)

Si necesitas regenerar los modelos desde la base de datos:

```bash
dotnet ef dbcontext scaffold "Server=localhost;Database=UsersDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c UsersDbContext --context-dir Data --force
```

---

## 5. Endpoints de la API

### 5.1 Autenticación (`/api/auth`)

| Método | Ruta                          | Descripción                              | Auth |
|--------|-------------------------------|------------------------------------------|------|
| POST   | `/api/auth/register`          | Registrar nuevo usuario (rol User)       | No   |
| POST   | `/api/auth/login`             | Login → JWT + refreshToken + redirectUrl | No   |
| POST   | `/api/auth/refresh`           | Renovar token con refresh token          | No   |
| POST   | `/api/auth/logout`            | Revocar refresh token                    | Sí   |
| POST   | `/api/auth/change-password`   | Cambiar contraseña                       | Sí   |

### 5.2 Usuarios (`/api/users`) — Requiere JWT

| Método | Ruta                        | Descripción                        | Rol    |
|--------|-----------------------------|------------------------------------|--------|
| GET    | `/api/users`                | Listar con paginación              | Todos  |
| GET    | `/api/users/me`             | Datos del usuario actual           | Todos  |
| GET    | `/api/users/{id}`           | Obtener por ID                     | Todos  |
| POST   | `/api/users`                | Crear usuario                      | Admin  |
| PUT    | `/api/users/{id}`           | Actualizar usuario                 | Todos  |
| DELETE | `/api/users/{id}`           | Soft delete (Activo=false)         | Admin  |
| PUT    | `/api/users/{id}/block`     | Bloquear/desbloquear               | Admin  |

### 5.3 Auditoría (`/api/audit`) — Solo Admin

| Método | Ruta                         | Descripción                      |
|--------|------------------------------|----------------------------------|
| GET    | `/api/audit/logs`            | Logs de auditoría con paginación |
| GET    | `/api/audit/login-attempts`  | Intentos de login con paginación |

---

## 6. Ejemplos de Peticiones

### Registrar usuario

```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Juan",
    "apellido": "Pérez",
    "email": "juan@correo.com",
    "password": "MiPassword1!",
    "confirmPassword": "MiPassword1!"
  }'
```

### Login

```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@sistema.com",
    "password": "Admin123!"
  }'
```

**Respuesta:**
```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64_refresh_token...",
    "redirectUrl": "/admin/dashboard",
    "usuario": {
      "id": "guid-aquí",
      "nombre": "Admin Sistema",
      "email": "admin@sistema.com",
      "rol": "Admin"
    }
  },
  "errors": []
}
```

### Listar usuarios (con JWT)

```bash
curl -X GET "https://localhost:5001/api/users?page=1&pageSize=10" \
  -H "Authorization: Bearer TU_TOKEN_JWT"
```

### Refresh token

```bash
curl -X POST https://localhost:5001/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "tu_refresh_token_aquí"
  }'
```

### Cambiar contraseña

```bash
curl -X POST https://localhost:5001/api/auth/change-password \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN_JWT" \
  -d '{
    "currentPassword": "Admin123!",
    "newPassword": "NuevaPass1!",
    "confirmNewPassword": "NuevaPass1!"
  }'
```

---

## 7. Credenciales de Prueba

| Email              | Password    | Rol   |
|--------------------|-------------|-------|
| admin@sistema.com  | Admin123!   | Admin |

> Registra este usuario usando el endpoint `/api/auth/register` y luego cambia su rol en la BD, o insértalo directamente con un hash BCrypt.

---

## 8. Estructura del Proyecto

```
Interview_Base/
├── Controllers/         → AuthController, UsersController, AuditController
├── Services/            → AuthService, UserService, AuditService
│   └── Interfaces/      → IAuthService, IUserService, IAuditService
├── Repositories/        → UserRepository, AuditRepository
│   └── Interfaces/      → IUserRepository, IAuditRepository
├── Models/              → Entidades EF (scaffold — NO modificar)
├── DTOs/                → Auth/, User/, Common/
├── Data/                → UsersDbContext
├── Middleware/           → ExceptionHandling, Audit
├── Helpers/             → JwtHelper, PasswordHelper
├── Extensions/          → ServiceExtensions (DI)
├── Validators/          → FluentValidation validators
├── Program.cs           → Entry point con toda la configuración
├── appsettings.json     → Configuración de producción
└── README.md            → Este archivo
```

---

## 9. Características Técnicas

- **JWT Bearer** con refresh tokens almacenados en BD
- **BCrypt** (workFactor=11) para hash de contraseñas
- **FluentValidation** para validación de DTOs
- **Middleware global** de excepciones con respuestas estandarizadas
- **Middleware de auditoría** automática
- **Soft delete** (campo `Activo`)
- **Bloqueo automático** tras 5 intentos fallidos (configurable)
- **Serilog** para logging a consola y archivos
- **Swagger/OpenAPI** con botón Authorize para Bearer Token
- **CORS** configurado (permisivo para desarrollo)
- **Async/await** en todas las operaciones de BD
- **Paginación** en listados

---

## 10. Paquetes NuGet

```
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Authentication.JwtBearer
BCrypt.Net-Next
FluentValidation.AspNetCore
Swashbuckle.AspNetCore
Serilog.AspNetCore
```
