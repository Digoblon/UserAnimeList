import type {
  AddAnimeListEntryRequest,
  AnimeListResponse,
  AnimeSearchResponse,
  ApiErrorPayload,
  ListFilter,
  RegisterUserRequest,
  Tokens,
  UpdateAnimeListEntryRequest,
  UserProfile
} from '../types/api';

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? '/api';

export class ApiError extends Error {
  status: number;
  tokenIsExpired: boolean;

  constructor(message: string, status: number, tokenIsExpired = false) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.tokenIsExpired = tokenIsExpired;
  }
}

function toQueryString(filters: ListFilter): string {
  const query = new URLSearchParams();

  Object.entries(filters).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      query.set(key, String(value));
    }
  });

  const serialized = query.toString();
  return serialized.length ? `?${serialized}` : '';
}

function parseError(payload: unknown, fallback: string): { message: string; tokenIsExpired: boolean } {
  if (payload && typeof payload === 'object') {
    const parsed = payload as ApiErrorPayload;
    if (Array.isArray(parsed.errors) && parsed.errors.length) {
      return { message: parsed.errors[0], tokenIsExpired: !!parsed.tokenIsExpired };
    }
    if (typeof parsed.message === 'string') {
      return { message: parsed.message, tokenIsExpired: !!parsed.tokenIsExpired };
    }

    return { message: fallback, tokenIsExpired: !!parsed.tokenIsExpired };
  }

  if (typeof payload === 'string' && payload.length) {
    return { message: payload, tokenIsExpired: false };
  }

  return { message: fallback, tokenIsExpired: false };
}

async function request<T>(path: string, init?: RequestInit, token?: string): Promise<T> {
  const headers = new Headers(init?.headers);

  if (token) headers.set('Authorization', `Bearer ${token}`);
  if (init?.body && !(init.body instanceof FormData)) headers.set('Content-Type', 'application/json');

  const response = await fetch(`${API_BASE}${path}`, { ...init, headers });

  if (response.status === 204) return {} as T;

  const contentType = response.headers.get('content-type') ?? '';
  const isJson = contentType.includes('application/json');
  const payload = isJson ? await response.json() : await response.text();

  if (!response.ok) {
    const parsed = parseError(payload, 'Erro ao processar a requisição.');
    throw new ApiError(parsed.message, response.status, parsed.tokenIsExpired);
  }

  return payload as T;
}

export const api = {
  login: (login: string, password: string) =>
    request<Tokens>('/login', {
      method: 'POST',
      body: JSON.stringify({ login, password })
    }),

  register: (input: RegisterUserRequest) =>
    request('/user', {
      method: 'POST',
      body: JSON.stringify(input)
    }),

  refreshToken: (refreshToken: string) =>
    request<Tokens>('/token/refresh-token', {
      method: 'POST',
      body: JSON.stringify({ refreshToken })
    }),

  getProfile: (token: string) => request<UserProfile>('/user', { method: 'GET' }, token),

  searchAnime: (query: string) =>
    request<AnimeSearchResponse>('/anime/search', {
      method: 'POST',
      body: JSON.stringify({ query })
    }),

  getMyList: (token: string, filters: ListFilter = {}) =>
    request<AnimeListResponse>(`/animelist/me/list${toQueryString(filters)}`, { method: 'GET' }, token),

  addToList: (token: string, input: AddAnimeListEntryRequest) =>
    request('/animelist', {
      method: 'POST',
      body: JSON.stringify(input)
    }, token),

  updateListEntry: (token: string, entryId: string, input: UpdateAnimeListEntryRequest) =>
    request(`/animelist/${entryId}`, {
      method: 'PUT',
      body: JSON.stringify(input)
    }, token),

  deleteListEntry: (token: string, entryId: string) =>
    request(`/animelist/${entryId}`, {
      method: 'DELETE'
    }, token)
};
