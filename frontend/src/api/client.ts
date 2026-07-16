import axios from "axios";
import type { AxiosError, InternalAxiosRequestConfig } from "axios";
import type { LoginResponse } from "../types/api";
import {
  getAccessToken,
  getRefreshToken,
  setAccessToken,
  clearSession,
  REFRESH_TOKEN_KEY,
} from "./tokenStorage";

// Base URL configurável via variável de ambiente (.env / .env.example -> VITE_API_URL)
const baseURL = import.meta.env.VITE_API_URL ?? "http://localhost:5000";

export const apiClient = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Interceptor: anexa o token JWT (armazenado no localStorage) em toda requisição.
apiClient.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Requisição de refresh usa uma instância axios "crua" (sem os interceptors acima),
// para não disparar o próprio interceptor de resposta 401 recursivamente e para não
// depender de src/api/auth.ts (evita import circular: auth.ts -> client.ts).
async function requestNewAccessToken(refreshTokenValue: string): Promise<LoginResponse> {
  const response = await axios.post<LoginResponse>(
    `${baseURL}/api/auth/refresh-token`,
    { refreshToken: refreshTokenValue },
  );
  return response.data;
}

function redirectToLogin(): void {
  clearSession();
  if (typeof window !== "undefined") {
    window.location.href = "/login";
  }
}

// Extensão local do config do Axios para marcar requisições já reprocessadas
// após um refresh, evitando loop infinito caso o refresh "funcione" mas o
// recurso continue retornando 401.
interface RetryableRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

// Evita múltiplas chamadas concorrentes de refresh-token quando várias
// requisições falham com 401 ao mesmo tempo: todas aguardam a mesma promise.
let refreshInFlight: Promise<string> | null = null;

// Interceptor de resposta: trata expiração do access token (60 min) tentando
// renovar via refresh token uma única vez; se falhar, desloga e manda pro login.
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as RetryableRequestConfig | undefined;

    if (error.response?.status !== 401 || !originalRequest || originalRequest._retry) {
      return Promise.reject(error);
    }

    const storedRefreshToken = getRefreshToken();
    if (!storedRefreshToken) {
      redirectToLogin();
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    try {
      if (!refreshInFlight) {
        refreshInFlight = requestNewAccessToken(storedRefreshToken).then((data) => {
          setAccessToken(data.accessToken);
          localStorage.setItem(REFRESH_TOKEN_KEY, data.refreshToken);
          return data.accessToken;
        });
      }
      const newAccessToken = await refreshInFlight;
      refreshInFlight = null;

      originalRequest.headers = originalRequest.headers ?? {};
      originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
      return apiClient(originalRequest);
    } catch (refreshError) {
      refreshInFlight = null;
      redirectToLogin();
      return Promise.reject(refreshError);
    }
  },
);

export default apiClient;
