# ── Build stage ──────────────────────────────────────
FROM node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json* ./
RUN npm ci

COPY . .

# ── Test stage (se ejecuta con --target test) ────────
FROM build AS test
RUN npx vitest run

# ── Production build ─────────────────────────────────
FROM build AS publish
RUN npm run build

# ── Nginx runtime ────────────────────────────────────
FROM nginx:alpine AS final
COPY --from=publish /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget -qO- http://localhost/ || exit 1
