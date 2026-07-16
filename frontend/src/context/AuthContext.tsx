import { createContext, useContext, useState, useCallback, useMemo } from "react";
import type { ReactNode } from "react";
import { login as loginRequest } from "../api/auth";
import { TOKEN_KEY, REFRESH_TOKEN_KEY, USER_KEY, clearSession } from "../api/tokenStorage";

interface AuthUser {
  userId: number;
  userName: string;
}

interface AuthContextValue {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (userName: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

// Decisão de design: o token é guardado em memória (estado do React) e também
// persistido no localStorage. Isso evita deslogar o usuário a cada refresh de
// página (F5), o que seria uma experiência ruim em um dashboard administrativo.
// Trade-off: localStorage é acessível via JavaScript no browser (risco de XSS),
// mas para um projeto interno/desenvolvimento é uma escolha simples e comum;
// numa evolução futura, cookies httpOnly seriam mais seguros.
export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem(TOKEN_KEY));
  const [user, setUser] = useState<AuthUser | null>(() => {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? (JSON.parse(raw) as AuthUser) : null;
  });

  const login = useCallback(async (userName: string, password: string) => {
    const response = await loginRequest(userName, password);
    const authUser: AuthUser = { userId: response.userId, userName: response.userName };

    localStorage.setItem(TOKEN_KEY, response.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(authUser));

    setToken(response.accessToken);
    setUser(authUser);
  }, []);

  const logout = useCallback(() => {
    clearSession();
    setToken(null);
    setUser(null);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({ user, token, isAuthenticated: !!token, login, logout }),
    [user, token, login, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth deve ser usado dentro de um AuthProvider");
  }
  return context;
}
