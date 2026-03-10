# ── Build stage ──────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore
COPY Interview_Base/Interview_Base.csproj Interview_Base/
COPY Interview_Base.Tests/Interview_Base.Tests.csproj Interview_Base.Tests/
COPY Interview_Base.sln .
RUN dotnet restore

# Copy all source
COPY . .

# Build
WORKDIR /src/Interview_Base
RUN dotnet build -c Release -o /app/build --no-restore

# ── Test stage (se ejecuta con --target test) ────────
FROM build AS test
WORKDIR /src
RUN dotnet test Interview_Base.Tests -c Release --no-restore --verbosity normal

# ── Publish stage ────────────────────────────────────
FROM build AS publish
WORKDIR /src/Interview_Base
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false --no-restore

# ── Runtime stage ────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Install curl for healthcheck
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Health check
HEALTHCHECK --interval=15s --timeout=5s --start-period=30s --retries=5 \
  CMD curl -f http://localhost:8080/swagger/index.html || exit 1

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Interview_Base.dll"]