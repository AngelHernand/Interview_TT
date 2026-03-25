#!/bin/bash
# Esperar a que SQL Server esté listo antes de ejecutar el script
echo "Esperando a que SQL Server inicie..."

# El host es 'sqlserver' (nombre del servicio en docker-compose)
SQL_HOST="${SQL_HOST:-sqlserver}"

for i in {1..60}; do
    /opt/mssql-tools18/bin/sqlcmd -S "$SQL_HOST" -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" > /dev/null 2>&1
    if [ $? -eq 0 ]; then
        echo "SQL Server está listo. Ejecutando script de inicialización..."
        /opt/mssql-tools18/bin/sqlcmd -S "$SQL_HOST" -U sa -P "$SA_PASSWORD" -C -i /docker-entrypoint-initdb/init-db.sql
        echo "Script init-db.sql ejecutado."

        echo "Ejecutando migración de interview sessions..."
        /opt/mssql-tools18/bin/sqlcmd -S "$SQL_HOST" -U sa -P "$SA_PASSWORD" -C -i /docker-entrypoint-initdb/migration-interview-sessions.sql
        echo "Migración ejecutada."
        exit 0
    fi
    echo "Intento $i/60 - SQL Server aún no está listo..."
    sleep 2
done

echo "ERROR: SQL Server no respondió a tiempo."
exit 1
