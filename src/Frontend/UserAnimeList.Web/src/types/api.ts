export type Nullable<T> = T | null;

export interface ApiErrorPayload {
  errors?: string[];
  tokenIsExpired?: boolean;
  message?: string;
}

export interface Tokens {
  accessToken: string;
  refreshToken: string;
}

export interface RegisterUserRequest {
  userName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface UserProfile {
  userName: string;
  email: string;
  imageUrl: string;
}

export interface Anime {
  id: string;
  name: string;
  imageUrl: string;
  score: Nullable<number>;
  status: number;
  type: number;
  airedFrom: string;
  airedUntil: string;
}

export interface AnimeSearchResponse {
  animes: Anime[];
}

export interface AnimeListEntry {
  id: string;
  name: string;
  status: number;
  score: Nullable<number>;
  progress: Nullable<number>;
}

export interface AnimeListResponse {
  lists: AnimeListEntry[];
}

export interface AddAnimeListEntryRequest {
  animeId: string;
  status: number;
  score?: Nullable<number>;
  progress?: Nullable<number>;
  dateStarted?: string;
  dateFinished?: string;
}

export interface UpdateAnimeListEntryRequest {
  status: number;
  score?: Nullable<number>;
  progress?: Nullable<number>;
  dateStarted?: string;
  dateFinished?: string;
}

export interface ListFilter {
  query?: string;
  status?: number;
  sortField?: number;
  sortDirection?: number;
}
