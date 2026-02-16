import { createContext, useContext, useMemo, useState } from 'react';
import { api } from '../api/client';
import { authStore, getRoleFromJwt } from '../utils/auth';
import type { RequestLoginJson, RequestRegisterUserJson } from '../types/contracts';

type AuthContextType = {
  accessToken: string;
  refreshToken: string;
  role: string | null;
  isLogged: boolean;
  isAdmin: boolean;
  login: (data: RequestLoginJson) => Promise<void>;
  register: (data: RequestRegisterUserJson) => Promise<void>;
  logout: () => void;
  refresh: () => Promise<void>;
};

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [accessToken, setAccessToken] = useState(authStore.getAccess());
  const [refreshToken, setRefreshToken] = useState(authStore.getRefresh());

  const role = useMemo(() => getRoleFromJwt(accessToken), [accessToken]);

  async function login(data: RequestLoginJson) {
    const response = await api.login(data);
    setAccessToken(response.tokens.accessToken);
    setRefreshToken(response.tokens.refreshToken);
    authStore.save(response.tokens.accessToken, response.tokens.refreshToken);
  }

  async function register(data: RequestRegisterUserJson) {
    const response = await api.registerUser(data);
    setAccessToken(response.tokens.accessToken);
    setRefreshToken(response.tokens.refreshToken);
    authStore.save(response.tokens.accessToken, response.tokens.refreshToken);
  }

  async function refresh() {
    if (!refreshToken) return;
    const response = await api.refreshToken({ refreshToken });
    setAccessToken(response.accessToken);
    setRefreshToken(response.refreshToken);
    authStore.save(response.accessToken, response.refreshToken);
  }

  function logout() {
    setAccessToken('');
    setRefreshToken('');
    authStore.clear();
  }

  return (
    <AuthContext.Provider value={{
      accessToken,
      refreshToken,
      role,
      isLogged: Boolean(accessToken),
      isAdmin: role?.toLowerCase() === 'admin',
      login,
      register,
      logout,
      refresh
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth outside provider');
  return ctx;
}
