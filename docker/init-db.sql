-- =====================================================
-- Script de inicialización de la base de datos UsersDB
-- Se ejecuta solo si la BD no existe aún
-- =====================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'UsersDB')
BEGIN
    CREATE DATABASE UsersDB;
END
GO

USE UsersDB;
GO

-- ── Tabla Roles ─────────────────────────────────────
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Roles')
BEGIN
    CREATE TABLE Roles (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        Nombre      NVARCHAR(50)  NOT NULL,
        Descripcion NVARCHAR(200) NULL,
        CONSTRAINT UQ_Roles_Nombre UNIQUE (Nombre)
    );

    -- Datos iniciales
    INSERT INTO Roles (Nombre, Descripcion) VALUES
        ('Admin', 'Administrador del sistema'),
        ('User',  'Usuario estándar');
END
GO

-- ── Tabla Usuarios ──────────────────────────────────
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios (
        Id                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        Nombre              NVARCHAR(100) NOT NULL,
        Email               NVARCHAR(255) NOT NULL,
        PasswordHash        NVARCHAR(500) NOT NULL,
        RolId               INT           NOT NULL,
        Activo              BIT           NOT NULL DEFAULT 1,
        Bloqueado           BIT           NOT NULL DEFAULT 0,
        FechaCreacion       DATETIME      NOT NULL DEFAULT GETDATE(),
        FechaActualizacion  DATETIME      NULL,
        UltimoAcceso        DATETIME      NULL,
        IntentosLogin       INT           NOT NULL DEFAULT 0,
        FechaBloqueo        DATETIME      NULL,
        CONSTRAINT UQ_Usuarios_Email UNIQUE (Email),
        CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolId) REFERENCES Roles(Id)
    );

    -- Usuario admin por defecto  (password: Admin123!)
    INSERT INTO Usuarios (Id, Nombre, Email, PasswordHash, RolId, Activo, Bloqueado, IntentosLogin)
    VALUES (
        NEWID(),
        'Administrador del Sistema',
        'admin@sistema.com',
        '$2a$11$1Vp88yPDzp1TIe56lBHsX.2NKzOkF7MIRa/DyewjDewlRwDfa7xeK',
        1, 1, 0, 0
    );
END
GO

-- ── Tabla RefreshTokens ─────────────────────────────
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RefreshTokens')
BEGIN
    CREATE TABLE RefreshTokens (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId       UNIQUEIDENTIFIER NOT NULL,
        Token           NVARCHAR(500) NOT NULL,
        FechaExpiracion DATETIME      NOT NULL,
        FechaCreacion   DATETIME      NOT NULL DEFAULT GETDATE(),
        Revocado        BIT           NOT NULL DEFAULT 0,
        FechaRevocacion DATETIME      NULL,
        ReemplazadoPor  NVARCHAR(500) NULL,
        DireccionIP     NVARCHAR(50)  NULL,
        UserAgent       NVARCHAR(500) NULL,
        CONSTRAINT FK_RefreshTokens_Usuarios FOREIGN KEY (UsuarioId)
            REFERENCES Usuarios(Id) ON DELETE CASCADE
    );
END
GO

-- ── Tabla AuditLogs ─────────────────────────────────
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AuditLogs')
BEGIN
    CREATE TABLE AuditLogs (
        Id               BIGINT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId        UNIQUEIDENTIFIER NULL,
        Accion           NVARCHAR(100) NOT NULL,
        Entidad          NVARCHAR(100) NULL,
        EntidadId        NVARCHAR(100) NULL,
        ValoresAnteriores NVARCHAR(MAX) NULL,
        ValoresNuevos    NVARCHAR(MAX) NULL,
        DireccionIP      NVARCHAR(50)  NULL,
        UserAgent        NVARCHAR(500) NULL,
        Fecha            DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_AuditLogs_Usuarios FOREIGN KEY (UsuarioId)
            REFERENCES Usuarios(Id) ON DELETE SET NULL
    );
END
GO

-- ── Tabla LoginAttempts ─────────────────────────────
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LoginAttempts')
BEGIN
    CREATE TABLE LoginAttempts (
        Id          BIGINT IDENTITY(1,1) PRIMARY KEY,
        Email       NVARCHAR(255) NOT NULL,
        Exitoso     BIT           NOT NULL,
        DireccionIP NVARCHAR(50)  NULL,
        UserAgent   NVARCHAR(500) NULL,
        MensajeError NVARCHAR(500) NULL,
        Fecha       DATETIME      NOT NULL DEFAULT GETDATE()
    );
END
GO

-- ── Vista vw_UsuariosActivos ────────────────────────
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_UsuariosActivos')
    DROP VIEW vw_UsuariosActivos;
GO

CREATE VIEW vw_UsuariosActivos AS
SELECT
    u.Id,
    u.Nombre,
    u.Email,
    r.Nombre AS Rol,
    u.Activo,
    u.FechaCreacion,
    u.UltimoAcceso
FROM Usuarios u
INNER JOIN Roles r ON u.RolId = r.Id
WHERE u.Activo = 1;
GO

PRINT 'Base de datos UsersDB inicializada correctamente.';
GO
