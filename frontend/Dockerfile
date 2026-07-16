# syntax=docker/dockerfile:1

# ==========================================================================
# Stage 1: build - instala dependencias e gera o bundle estatico (Vite)
# ==========================================================================
FROM node:20-alpine AS build
WORKDIR /app

# Copia apenas os manifestos primeiro para aproveitar o cache de layer do npm ci.
COPY package.json package-lock.json ./
RUN npm ci

# Copia o restante do codigo-fonte.
COPY . .

# VITE_API_URL precisa ser injetada em build-time: o Vite embute env vars
# no bundle JS durante o "npm run build" (nao existe leitura em runtime).
ARG VITE_API_URL=http://localhost:5000
ENV VITE_API_URL=$VITE_API_URL

RUN npm run build

# ==========================================================================
# Stage 2: final - serve os arquivos estaticos via Nginx
# ==========================================================================
FROM nginx:alpine AS final

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
