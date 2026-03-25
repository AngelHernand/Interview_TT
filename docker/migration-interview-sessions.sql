-- =====================================================
-- Migración: Tablas para InterviewService
-- Ejecutar sobre UsersDB después de init-db.sql
-- =====================================================

USE UsersDB;
GO

-- ── Tabla InterviewSessions ───────────────────────────
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'InterviewSessions')
BEGIN
    CREATE TABLE InterviewSessions (
        Id                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        UsuarioId           UNIQUEIDENTIFIER NOT NULL,
        TipoEntrevista      NVARCHAR(20)     NOT NULL,  -- tecnica, behavioral, mixta
        Tecnologia          NVARCHAR(50)     NULL,
        Nivel               NVARCHAR(20)     NOT NULL,  -- Junior, Mid, Senior
        Estado              NVARCHAR(20)     NOT NULL DEFAULT 'EnCurso', -- EnCurso, Finalizada, Cancelada
        SystemPrompt        NVARCHAR(MAX)    NOT NULL,
        ContextoRecuperado  NVARCHAR(MAX)    NULL,
        FechaInicio         DATETIME         NOT NULL DEFAULT GETDATE(),
        FechaFin            DATETIME         NULL,
        EvaluacionJson      NVARCHAR(MAX)    NULL,
        DuracionMinutos     INT              NOT NULL DEFAULT 30,
        CONSTRAINT FK_InterviewSessions_Usuarios FOREIGN KEY (UsuarioId)
            REFERENCES Usuarios(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_InterviewSessions_UsuarioId ON InterviewSessions(UsuarioId);
    CREATE INDEX IX_InterviewSessions_Estado ON InterviewSessions(Estado);

    PRINT 'Tabla InterviewSessions creada.';
END
GO

-- ── Tabla InterviewMessages ───────────────────────────
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'InterviewMessages')
BEGIN
    CREATE TABLE InterviewMessages (
        Id              BIGINT IDENTITY(1,1) PRIMARY KEY,
        SessionId       UNIQUEIDENTIFIER NOT NULL,
        Rol             NVARCHAR(20)     NOT NULL,  -- system, entrevistador, candidato
        Contenido       NVARCHAR(MAX)    NOT NULL,
        Orden           INT              NOT NULL,
        FechaCreacion   DATETIME         NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_InterviewMessages_Sessions FOREIGN KEY (SessionId)
            REFERENCES InterviewSessions(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_InterviewMessages_SessionId_Orden ON InterviewMessages(SessionId, Orden);

    PRINT 'Tabla InterviewMessages creada.';
END
GO

PRINT 'Migración InterviewSessions completada.';
GO
