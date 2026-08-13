import type { AuthResponse, User } from "../types";

const AUTH_KEY = "eduassign_auth";

export const saveAuth = (auth: AuthResponse) =>
  localStorage.setItem(AUTH_KEY, JSON.stringify(auth));

export const getAuth = (): AuthResponse | null => {
  try {
    const raw = localStorage.getItem(AUTH_KEY);
    return raw ? JSON.parse(raw) as AuthResponse : null;
  } catch {
    return null;
  }
};

export const updateAuthTokens = (accessToken: string, refreshToken: string) => {
  const auth = getAuth();
  if (!auth) return;
  saveAuth({ ...auth, accessToken, refreshToken });
};

export const clearAuth = () => localStorage.removeItem(AUTH_KEY);

export const getToken = () => getAuth()?.accessToken ?? null;
export const getRefreshToken = () => getAuth()?.refreshToken ?? null;
export const getUser = (): User | null => getAuth()?.user ?? null;
