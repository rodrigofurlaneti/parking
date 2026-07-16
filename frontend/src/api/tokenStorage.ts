// Chaves de persistência da sessão no localStorage, compartilhadas entre o
// AuthContext (árvore React) e o interceptor de resposta do apiClient (fora
// da árvore React, não pode usar hooks/contexto diretamente).
export const TOKEN_KEY = "parking.accessToken";
export const REFRESH_TOKEN_KEY = "parking.refreshToken";
export const USER_KEY = "parking.user";

export function getAccessToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function getRefreshToken(): string | null {
  return localStorage.getItem(REFRESH_TOKEN_KEY);
}

export function setAccessToken(accessToken: string): void {
  localStorage.setItem(TOKEN_KEY, accessToken);
}

export function clearSession(): void {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}
