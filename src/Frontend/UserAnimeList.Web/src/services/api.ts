import {
  ApiResult,
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

const API_BASE = '/api';

export class ApiError extends Error {
  status: number;
  payload?: ResponseErrorJson;

  constructor(status: number, message: string, payload?: ResponseErrorJson) {
    super(message);
    this.status = status;
    this.payload = payload;
  }
}

function toQuery(filters: RequestAnimeListEntryFilterJson): string {
  const params = new URLSearchParams();
  Object.entries(filters).forEach(([k, v]) => {
    if (v !== undefined && v !== null && v !== '') params.set(k, String(v));
  });
  const s = params.toString();
  return s ? `?${s}` : '';
}

async function request<T>(path: string, init: RequestInit = {}): Promise<ApiResult<T>> {
  const res = await fetch(`${API_BASE}${path}`, init);
  if (res.status === 204) return { data: {} as T, status: 204 };
  const isJson = res.headers.get('content-type')?.includes('application/json');
  const body = isJson ? await res.json() : undefined;
  if (!res.ok) {
    throw new ApiError(res.status, body?.errors?.join(' | ') ?? res.statusText, body as ResponseErrorJson | undefined);
  }
  return { data: body as T, status: res.status };
}

export class ApiClient {
  token: string | null = null;

  setToken(token: string | null) { this.token = token; }

  private authHeaders(extra?: HeadersInit): HeadersInit {
    return { ...(extra ?? {}), ...(this.token ? { Authorization: `Bearer ${this.token}` } : {}) };
  }

  login(payload: RequestLoginJson) {
    return request<ResponseRegisteredUserJson>('/login', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  }

  refreshToken(payload: RequestNewTokenJson) {
    return request<ResponseTokensJson>('/token/refresh-token', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  }

  registerUser(payload: RequestRegisterUserJson) {
    return request<ResponseRegisteredUserJson>('/user', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  }

  getUserProfile() {
    return request<ResponseUserProfileJson>('/user', { headers: this.authHeaders() });
  }

  updateUser(payload: RequestUpdateUserJson) {
    return request<void>('/user', { method: 'PUT', headers: this.authHeaders({ 'Content-Type': 'application/json' }), body: JSON.stringify(payload) });
  }

  changePassword(payload: RequestChangePasswordJson) {
    return request<ResponseChangePasswordJson>('/user/change-password', { method: 'PUT', headers: this.authHeaders({ 'Content-Type': 'application/json' }), body: JSON.stringify(payload) });
  }

  updateUserImage(file: File) {
    const form = new FormData();
    form.append('image', file);
    return request<ResponseUpdateImageJson>('/user/me/image', { method: 'PUT', headers: this.authHeaders(), body: form });
  }

  deleteUserImage() { return request<void>('/user/me/image', { method: 'DELETE', headers: this.authHeaders() }); }
  deleteMe() { return request<void>('/user/me', { method: 'DELETE', headers: this.authHeaders() }); }

  searchAnime(payload: RequestAnimeSearchJson) {
    return request<ResponseAnimesJson>('/anime/search', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  }

  getAnimeById(id: string) { return request<ResponseAnimeJson>(`/anime/${id}`); }
  registerAnime(payload: RequestAnimeJson) { return request<ResponseRegisteredAnimeJson>('/anime', { method: 'POST', headers: this.authHeaders({ 'Content-Type': 'application/json' }), body: JSON.stringify(payload) }); }
  updateAnime(id: string, payload: RequestAnimeJson) { return request<void>(`/anime/${id}`, { method: 'PUT', headers: this.authHeaders({ 'Content-Type': 'application/json' }), body: JSON.stringify(payload) }); }
  deleteAnime(id: string) { return request<void>(`/anime/${id}`, { method: 'DELETE', headers: this.authHeaders() }); }
  updateAnimeImage(id: string, file: File) {
    const form = new FormData(); form.append('image', file);
    return request<ResponseUpdateImageJson>(`/anime/${id}/image`, { method: 'PUT', headers: this.authHeaders(), body: form });
  }
  deleteAnimeImage(id: string) { return request<void>(`/anime/${id}/image`, { method: 'DELETE', headers: this.authHeaders() }); }

  addAnimeListEntry(payload: RequestAnimeListEntryJson) { return request<ResponseAnimeListEntryJson>('/animelist', { method: 'POST', headers: this.authHeaders({ 'Content-Type': 'application/json' }), body: JSON.stringify(payload) }); }
  getAnimeListEntryById(id: string) { return request<ResponseAnimeListEntryJson>(`/animelist/${id}`, { headers: this.authHeaders() }); }
  listMyAnimeList(filters: RequestAnimeListEntryFilterJson) { return request<ResponseAnimeListsJson>(`/animelist/me/list${toQuery(filters)}`, { headers: this.authHeaders() }); }
  listUserAnimeList(userId: string, filters: RequestAnimeListEntryFilterJson) { return request<ResponseAnimeListsJson>(`/animelist/list/${userId}${toQuery(filters)}`); }
  updateAnimeListEntry(id: string, payload: RequestUpdateAnimeListEntryJson) { return request<void>(`/animelist/${id}`, { method: 'PUT', headers: this.authHeaders({ 'Content-Type': 'application/json' }), body: JSON.stringify(payload) }); }
  deleteAnimeListEntry(id: string) { return request<void>(`/animelist/${id}`, { method: 'DELETE', headers: this.authHeaders() }); }

  registerGenre(payload: RequestRegisterGenreJson) { return request<ResponseRegisteredGenreJson>('/genre', { method: 'POST', headers: this.authHeaders({ 'Content-Type': 'application/json' }), body: JSON.stringify(payload) }); }
  getGenreById(id: string) { return request<ResponseGenreJson>(`/genre/${id}`); }
  searchGenre(payload: RequestGenreGetByNameJson) { return request<ResponseGenresJson>('/genre/search', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) }); }
  updateGenre(id: string, payload: RequestUpdateGenreJson) { return request<void>(`/genre/${id}`, { method: 'PUT', headers: this.authHeaders({ 'Content-Type': 'application/json' }), body: JSON.stringify(payload) }); }
  deleteGenre(id: string) { return request<void>(`/genre/${id}`, { method: 'DELETE', headers: this.authHeaders() }); }

  registerStudio(payload: RequestRegisterStudioJson) { return request<ResponseRegisteredStudioJson>('/studio', { method: 'POST', headers: this.authHeaders({ 'Content-Type': 'application/json' }), body: JSON.stringify(payload) }); }
  getStudioById(id: string) { return request<ResponseStudioJson>(`/studio/${id}`); }
  searchStudio(payload: RequestStudioGetByNameJson) { return request<ResponseStudiosJson>('/studio/search', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) }); }
  updateStudio(id: string, payload: RequestUpdateStudioJson) { return request<void>(`/studio/${id}`, { method: 'PUT', headers: this.authHeaders({ 'Content-Type': 'application/json' }), body: JSON.stringify(payload) }); }
  deleteStudio(id: string) { return request<void>(`/studio/${id}`, { method: 'DELETE', headers: this.authHeaders() }); }
}

export const api = new ApiClient();
