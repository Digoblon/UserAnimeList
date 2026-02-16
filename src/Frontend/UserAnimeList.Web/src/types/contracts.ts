export enum AnimeEntryStatus {
  Watching = 0,
  Completed = 1,
  PlanToWatch = 2,
  Dropped = 3,
  OnHold = 4
}

export enum AnimeListSort {
  Name = 0,
  Score = 1,
  Progress = 2,
  Status = 3,
  Type = 4,
  DateStarted = 5,
  DateFinished = 6
}

export enum AnimeSort {
  Name = 0,
  Episodes = 1,
  Status = 2,
  Type = 3,
  AiredFrom = 4,
  AiredUntil = 5,
  Premiered = 6
}

export enum AnimeStatus {
  Airing = 0,
  Finished = 1,
  NotYetAired = 2
}

export enum AnimeType {
  Tv = 0,
  Movie = 1,
  OVA = 2,
  ONA = 3,
  Special = 4,
  Unknown = 5
}

export enum Season {
  Spring = 0,
  Summer = 1,
  Fall = 2,
  Winter = 3
}

export enum SortDirection {
  Asc = 0,
  Desc = 1
}

export enum SourceType {
  Manga = 0,
  LightNovel = 1,
  Novel = 2,
  VisualNovel = 3,
  Game = 4,
  WebNovel = 5,
  OneShot = 6,
  Doujinshi = 7,
  Original = 8,
  Unknown = 9
}

export interface RequestAnimeSearchJson { query?: string }
export interface RequestRegisterGenreJson { name: string; description: string }
export interface RequestUpdateStudioJson { name: string; description: string }
export interface RequestLoginJson { login: string; password: string }
export interface RequestChangePasswordJson { password: string; newPassword: string; confirmNewPassword: string }
export interface RequestGenreGetByNameJson { name: string }
export interface RequestAnimeFilterJson {
  query?: string;
  status?: AnimeStatus;
  type?: AnimeType;
  genres?: string[];
  studios?: string[];
  airedFrom?: string;
  airedUntil?: string;
  premieredSeason?: Season;
  premieredYear?: number;
  sortField?: AnimeSort;
  sortDirection?: SortDirection;
}
export interface RequestAnimeListEntryFilterJson {
  query?: string;
  status?: AnimeEntryStatus;
  dateStarted?: string;
  dateFinished?: string;
  sortField?: AnimeListSort;
  sortDirection?: SortDirection;
}
export interface RequestAnimeJson {
  name: string;
  synopsis?: string;
  episodes?: number;
  genres: string[];
  studios: string[];
  status: AnimeStatus;
  source: SourceType;
  type: AnimeType;
  airedFrom?: string;
  airedUntil?: string;
}
export interface RequestUpdateAnimeListEntryJson {
  status: AnimeEntryStatus;
  score?: number | null;
  progress?: number | null;
  dateStarted?: string;
  dateFinished?: string;
}
export interface RequestNewTokenJson { refreshToken: string }
export interface RequestUpdateGenreJson { name: string; description: string }
export interface RequestStudioGetByNameJson { name: string }
export interface RequestUpdateImageFormData { image?: File | null }
export interface RequestAnimeListEntryJson {
  animeId: string;
  status: AnimeEntryStatus;
  score?: number | null;
  progress?: number | null;
  dateStarted?: string;
  dateFinished?: string;
}
export interface RequestRegisterUserJson {
  userName: string;
  email: string;
  password: string;
  confirmPassword: string;
}
export interface RequestUpdateUserJson { userName: string; email: string }
export interface RequestRegisterStudioJson { name: string; description: string }

export interface ResponseUpdateImageJson { imageUrl: string }
export interface ResponseGenresJson { genres: ResponseGenreJson[] }
export interface ResponseStudioJson { id: string; name: string; description: string }
export interface ResponseUserProfileJson { userName: string; email: string; imageUrl: string }
export interface ResponseChangePasswordJson { tokens: ResponseTokensJson }
export interface ResponseStudiosJson { studios: ResponseStudioJson[] }
export interface ResponseAnimeJson {
  id: string;
  name: string;
  imageUrl: string;
  score?: number | null;
  synopsis?: string;
  episodes?: number | null;
  genres: string[];
  studios: string[];
  status: AnimeStatus;
  source: SourceType;
  type: AnimeType;
  airedFrom?: string;
  airedUntil?: string;
  premiered: string;
}
export interface ResponseRegisteredStudioJson { name: string; description: string }
export interface ResponseGenreJson { id: string; name: string; description: string }
export interface ResponseRegisteredAnimeJson { animeId: string; name: string }
export interface ResponseTokensJson { accessToken: string; refreshToken: string }
export interface ResponseRegisteredGenreJson { name: string; description: string }
export interface ResponseAnimeListsJson { lists: ResponseShortAnimeListEntryJson[] }
export interface ResponseShortAnimeListEntryJson {
  id: string;
  name: string;
  status: AnimeEntryStatus;
  score?: number | null;
  progress?: number | null;
}
export interface ResponseShortAnimeJson {
  id: string;
  name: string;
  imageUrl: string;
  score?: number | null;
  status: AnimeStatus;
  type: AnimeType;
  airedFrom: string;
  airedUntil: string;
}
export interface ResponseErrorJson {
  code: string;
  message: string;
  errors: string[];
  traceId: string;
  tokenIsExpired: boolean;
}
export interface ResponseRegisteredUserJson { userName: string; tokens: ResponseTokensJson }
export interface ResponseAnimesJson { animes: ResponseShortAnimeJson[] }
export interface ResponseAnimeListEntryJson {
  id: string;
  animeId: string;
  status: AnimeEntryStatus;
  score?: number | null;
  progress?: number | null;
  dateStarted?: string;
  dateFinished?: string;
}
