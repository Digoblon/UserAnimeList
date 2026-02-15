import type {
  RequestAnimeFilterJson,
  RequestAnimeJson,
  RequestAnimeListEntryFilterJson,
  RequestAnimeListEntryJson,
  RequestAnimeSearchJson,
  RequestChangePasswordJson,
  RequestGenreGetByNameJson,
  RequestLoginJson,
  RequestNewTokenJson,
  RequestRegisterGenreJson,
  RequestRegisterStudioJson,
  RequestRegisterUserJson,
  RequestStudioGetByNameJson,
  RequestUpdateAnimeListEntryJson,
  RequestUpdateGenreJson,
  RequestUpdateImageFormData,
  RequestUpdateStudioJson,
  RequestUpdateUserJson,
  ResponseAnimeJson,
  ResponseAnimeListEntryJson,
  ResponseAnimeListsJson,
  ResponseAnimesJson,
  ResponseChangePasswordJson,
  ResponseErrorJson,
  ResponseGenreJson,
  ResponseGenresJson,
  ResponseRegisteredAnimeJson,
  ResponseRegisteredGenreJson,
  ResponseRegisteredStudioJson,
  ResponseRegisteredUserJson,
  ResponseStudioJson,
  ResponseStudiosJson,
  ResponseTokensJson,
  ResponseUpdateImageJson,
  ResponseUserProfileJson
} from '../types/contracts';

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? '/api';

export class ApiError extends Error {
  constructor(public status: number, public payload?: ResponseErrorJson | string) {
    super(typeof payload === 'string' ? payload : payload?.message || `HTTP ${status}`);
    this.name = 'ApiError';
  }
}

function query(params: object) {
  const search = new URLSearchParams();
  Object.entries(params as Record<string, unknown>).forEach(([k, v]) => {
    if (v === undefined || v === null || v === '') return;
    if (Array.isArray(v)) {
      v.forEach((item) => search.append(k, String(item)));
      return;
    }
    search.set(k, String(v));
  });
  const qs = search.toString();
  return qs ? `?${qs}` : '';
}

async function request<T>(path: string, init: RequestInit = {}, token?: string): Promise<T> {
  const headers = new Headers(init.headers);
  if (token) headers.set('Authorization', `Bearer ${token}`);

  const isForm = init.body instanceof FormData;
  if (init.body && !isForm) headers.set('Content-Type', 'application/json');

  const response = await fetch(`${API_BASE}${path}`, { ...init, headers });
  if (response.status === 204) return {} as T;

  const contentType = response.headers.get('content-type') || '';
  const isJson = contentType.includes('application/json');
  const payload = isJson ? await response.json() : await response.text();

  if (!response.ok) throw new ApiError(response.status, payload);
  return payload as T;
}

export const api = {
  login: (data: RequestLoginJson) => request<ResponseRegisteredUserJson>('/login', { method: 'POST', body: JSON.stringify(data) }),
  refreshToken: (data: RequestNewTokenJson) => request<ResponseTokensJson>('/token/refresh-token', { method: 'POST', body: JSON.stringify(data) }),

  registerUser: (data: RequestRegisterUserJson) => request<ResponseRegisteredUserJson>('/user', { method: 'POST', body: JSON.stringify(data) }),
  getUserProfile: (token: string) => request<ResponseUserProfileJson>('/user', {}, token),
  updateUser: (token: string, data: RequestUpdateUserJson) => request<void>('/user', { method: 'PUT', body: JSON.stringify(data) }, token),
  changePassword: (token: string, data: RequestChangePasswordJson) => request<ResponseChangePasswordJson>('/user/change-password', { method: 'PUT', body: JSON.stringify(data) }, token),
  deleteUserMe: (token: string) => request<void>('/user/me', { method: 'DELETE' }, token),
  updateUserImage: (token: string, data: RequestUpdateImageFormData) => {
    const form = new FormData();
    if (data.image) form.append('image', data.image);
    return request<ResponseUpdateImageJson>('/user/me/image', { method: 'PUT', body: form }, token);
  },
  deleteUserImage: (token: string) => request<void>('/user/me/image', { method: 'DELETE' }, token),

  registerAnime: (token: string, data: RequestAnimeJson) => request<ResponseRegisteredAnimeJson>('/anime', { method: 'POST', body: JSON.stringify(data) }, token),
  getAnimeById: (id: string) => request<ResponseAnimeJson>(`/anime/${id}`),
  searchAnime: (data: RequestAnimeSearchJson) => request<ResponseAnimesJson>(`/anime/search${query(data)}`),
  filterAnime: (data: RequestAnimeFilterJson) => request<ResponseAnimesJson>(`/anime/filter${query(data)}`),
  updateAnime: (token: string, id: string, data: RequestAnimeJson) => request<void>(`/anime/${id}`, { method: 'PUT', body: JSON.stringify(data) }, token),
  deleteAnime: (token: string, id: string) => request<void>(`/anime/${id}`, { method: 'DELETE' }, token),
  updateAnimeImage: (token: string, id: string, data: RequestUpdateImageFormData) => {
    const form = new FormData();
    if (data.image) form.append('image', data.image);
    return request<ResponseUpdateImageJson>(`/anime/${id}/image`, { method: 'PUT', body: form }, token);
  },
  deleteAnimeImage: (token: string, id: string) => request<void>(`/anime/${id}/image`, { method: 'DELETE' }, token),

  registerGenre: (token: string, data: RequestRegisterGenreJson) => request<ResponseRegisteredGenreJson>('/genre', { method: 'POST', body: JSON.stringify(data) }, token),
  getGenreById: (id: string) => request<ResponseGenreJson>(`/genre/${id}`),
  getGenreByName: (data: RequestGenreGetByNameJson) => request<ResponseGenresJson>('/genre/search', { method: 'POST', body: JSON.stringify(data) }),
  updateGenre: (token: string, id: string, data: RequestUpdateGenreJson) => request<void>(`/genre/${id}`, { method: 'PUT', body: JSON.stringify(data) }, token),
  deleteGenre: (token: string, id: string) => request<void>(`/genre/${id}`, { method: 'DELETE' }, token),

  registerStudio: (token: string, data: RequestRegisterStudioJson) => request<ResponseRegisteredStudioJson>('/studio', { method: 'POST', body: JSON.stringify(data) }, token),
  getStudioById: (id: string) => request<ResponseStudioJson>(`/studio/${id}`),
  getStudioByName: (data: RequestStudioGetByNameJson) => request<ResponseStudiosJson>('/studio/search', { method: 'POST', body: JSON.stringify(data) }),
  updateStudio: (token: string, id: string, data: RequestUpdateStudioJson) => request<void>(`/studio/${id}`, { method: 'PUT', body: JSON.stringify(data) }, token),
  deleteStudio: (token: string, id: string) => request<void>(`/studio/${id}`, { method: 'DELETE' }, token),

  addAnimeListEntry: (token: string, data: RequestAnimeListEntryJson) => request<ResponseAnimeListEntryJson>('/animelist', { method: 'POST', body: JSON.stringify(data) }, token),
  getAnimeListEntryById: (token: string, id: string) => request<ResponseAnimeListEntryJson>(`/animelist/${id}`, {}, token),
  listAnimeByUserId: (userId: string, data: RequestAnimeListEntryFilterJson) => request<ResponseAnimeListsJson>(`/animelist/list/${userId}${query(data)}`),
  listMyAnime: (token: string, data: RequestAnimeListEntryFilterJson) => request<ResponseAnimeListsJson>(`/animelist/me/list${query(data)}`, {}, token),
  updateAnimeListEntry: (token: string, id: string, data: RequestUpdateAnimeListEntryJson) => request<void>(`/animelist/${id}`, { method: 'PUT', body: JSON.stringify(data) }, token),
  deleteAnimeListEntry: (token: string, id: string) => request<void>(`/animelist/${id}`, { method: 'DELETE' }, token)
};
