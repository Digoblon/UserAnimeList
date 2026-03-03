const ACCESS_KEY = 'ual_access';
const REFRESH_KEY = 'ual_refresh';

export const authStore = {
  getAccess: () => localStorage.getItem(ACCESS_KEY) ?? '',
  getRefresh: () => localStorage.getItem(REFRESH_KEY) ?? '',
  save: (access: string, refresh: string) => {
    localStorage.setItem(ACCESS_KEY, access);
    localStorage.setItem(REFRESH_KEY, refresh);
  },
  clear: () => {
    localStorage.removeItem(ACCESS_KEY);
    localStorage.removeItem(REFRESH_KEY);
  }
};

export function getRoleFromJwt(token: string): string | null {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
  } catch {
    return null;
  }
}
